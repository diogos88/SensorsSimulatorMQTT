using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace diogos88.MQTT.SensorsSimulator
{
   public partial class Form1 : Form
   {
      private const string SETTINGS_FILENAME = "settings.cfg";
      private const int NUMBER_ITEMS_DISPLAYED = 15;

      private Settings m_Settings;
      private readonly Sensors m_Sensors;
      private readonly SensorsSettings m_SensorsSettings;
      private readonly CircularBuffer<MqttMsgPublishEventArgs> m_Messages;
      private MqttClient m_Client;
      private bool m_FillingControls;
      private Thread m_SenderThread;
      private CancellationTokenSource m_Cancel;
      private XmlAttributeOverrides m_XmlAttributeOverrides;
      private DynamicContractResolver m_DynamicContractResolver;
      private List<string> m_PropertiesToSend; 
      private TextFormat m_TextFormat;
      private int m_Interval;

      public Form1()
      {
         InitializeComponent();

         m_Settings = new Settings();
         m_Sensors = new Sensors();
         m_SensorsSettings = new SensorsSettings();
         m_Client = null;
         m_FillingControls = false;
         m_PropertiesToSend = new List<string>();

         m_Messages = new CircularBuffer<MqttMsgPublishEventArgs>(NUMBER_ITEMS_DISPLAYED);
         m_Messages.BufferContentChanged += BufferContentChangedHandler;

         m_TextFormat = TextFormat.JSON;
         m_Interval = 1000;

         UpdateDynamicSerializationAttributes();
      }

      private void UpdateDynamicSerializationAttributes()
      {
         var propertyInfos = typeof(Sensors).GetProperties(BindingFlags.Public | BindingFlags.Instance);
         m_PropertiesToSend.Clear();

         // XML
         m_XmlAttributeOverrides = new XmlAttributeOverrides();

         foreach (PropertyInfo propertyInfo in propertyInfos)
         {
            var ignore = true;
            switch (propertyInfo.Name)
            {
               case Sensors.DOOR_WINDOW:
                  ignore = !m_SensorsSettings.DoorWindowEnabled;
                  break;
               case Sensors.DIMMER:
                  ignore = !m_SensorsSettings.DimmerEnabled;
                  break;
               case Sensors.DISTANCE:
                  ignore = !m_SensorsSettings.DistanceEnabled;
                  break;
               case Sensors.DUST:
                  ignore = !m_SensorsSettings.DustEnabled;
                  break;
               case Sensors.GAS_LPG:
                  ignore = !m_SensorsSettings.GasEnabled;
                  break;
               case Sensors.GAS_CO2:
                  ignore = !m_SensorsSettings.GasEnabled;
                  break;
               case Sensors.GAS_SMOKE:
                  ignore = !m_SensorsSettings.GasEnabled;
                  break;
               case Sensors.HUMIDITY:
                  ignore = !m_SensorsSettings.HumidityEnabled;
                  break;
               case Sensors.LIGHT:
                  ignore = !m_SensorsSettings.LightEnabled;
                  break;
               case Sensors.MOTION:
                  ignore = !m_SensorsSettings.MotionEnabled;
                  break;
               case Sensors.PRESSURE:
                  ignore = !m_SensorsSettings.PressureEnabled;
                  break;
               case Sensors.RAIN_MOISTURE:
                  ignore = !m_SensorsSettings.RainMoistureEnabled;
                  break;
               case Sensors.TEMPERATURE:
                  ignore = !m_SensorsSettings.TemperatureEnabled;
                  break;
               case Sensors.UV:
                  ignore = !m_SensorsSettings.UVEnabled;
                  break;
            }

            m_XmlAttributeOverrides.Add(typeof(Sensors), propertyInfo.Name, new XmlAttributes { XmlIgnore = ignore });

            if (!ignore)
               m_PropertiesToSend.Add(propertyInfo.Name);
         }

         // JSON
         m_DynamicContractResolver = new DynamicContractResolver(m_PropertiesToSend);
         
      }

      private void Form1_Load(object sender, EventArgs e)
      {
         LoadSettings();
         FillControls();
         UpdateForm();

         Connect();

         m_Cancel = new CancellationTokenSource();
         m_SenderThread = new Thread(AsyncSenderThread);
         m_SenderThread.Start();
      }

      private void Connect()
      {
         if (m_Settings.Topic.Equals("#"))
            return;

         if (m_Client != null && m_Client.IsConnected)
         {
            m_Client.Disconnect();
            m_Client.MqttMsgPublishReceived -= MqttMsgPublishReceivedHandler;
         }

         m_Client = new MqttClient(m_Settings.BrokerIP);
         m_Client.ProtocolVersion = MqttProtocolVersion.Version_3_1; // Protocol v3

         try
         {
            if (m_Settings.Username.Equals("") && m_Settings.Password.Equals(""))
               m_Client.Connect(Guid.NewGuid().ToString());
            else
               m_Client.Connect(Guid.NewGuid().ToString(), m_Settings.Username, m_Settings.Password);

            m_Client.MqttMsgPublishReceived += MqttMsgPublishReceivedHandler;

            var sensors = Utilities.GetConstants(typeof (Sensors));
            var topics = new string[sensors.Count+1];
            var qosLevels = new byte[sensors.Count+1];

            topics[0] = m_Settings.Topic;
            qosLevels[0] = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE;
            for (int i=0; i< sensors.Count; i++)
            {
               topics[i+1] = m_Settings.Topic + "/" + sensors[i].ToLower();
               qosLevels[i+1] = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE;
            }
            m_Client.Subscribe(topics, qosLevels);
         }
         catch
         {
            m_Client = null;
         }
         finally
         {
            UpdateForm();
         }
      }

      private void MqttMsgPublishReceivedHandler(object sender, MqttMsgPublishEventArgs e)
      {
         m_Messages.Add(e);
      }

      private void LoadSettings()
      {
         if (!System.IO.File.Exists(SETTINGS_FILENAME))
            return;

         m_Settings = Utilities.Deserialize<Settings>(SETTINGS_FILENAME) ?? new Settings();
      }

      private void SaveSettings()
      {
         Utilities.Serialize(m_Settings, SETTINGS_FILENAME);
      }

      private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var settingsForm = new SettingsForm(m_Settings);
         if (settingsForm.ShowDialog(this) == DialogResult.OK)
         {
            m_Settings = settingsForm.SettingsCopy;
            SaveSettings();

            Connect();
         }
      }

      private void FillControls()
      {
         m_FillingControls = true;

         DoorWindowOpenRadioButton.Checked = m_Sensors.DoorWindow;
         DoorWindowCloseRadioButton.Checked = !m_Sensors.DoorWindow;
         DimmerTrackBar.Value = m_Sensors.Dimmer;
         DistanceTextbox.Text = m_Sensors.Distance.ToString();
         DustTextbox.Text = m_Sensors.Dust.ToString();
         LPGTextbox.Text = m_Sensors.GasLPG.ToString();
         CO2Textbox.Text = m_Sensors.GasCO2.ToString();
         SmokeTextbox.Text = m_Sensors.GasSmoke.ToString();
         HumidityTextbox.Text = m_Sensors.Humidity.ToString();
         LightTextbox.Text = m_Sensors.Light.ToString();
         MotionTrippedCheckBox.Checked = m_Sensors.Motion;
         PressureTextbox.Text = m_Sensors.Pressure.ToString();
         RainMoistureDetectedCheckBox.Checked = m_Sensors.RainMoisture;
         TemperatureTextbox.Text = m_Sensors.Temperature.ToString();

         foreach (var value in SensorsSettings.UV_INDEXES)
            UVComboBox.Items.Add(value);
         UVComboBox.SelectedIndex = m_Sensors.UVIndex;

         IntervalComboBox.Items.AddRange(new[] { "50", "100", "250", "500", "1000", "2500", "5000", "10000", "30000", "60000" });
         IntervalComboBox.SelectedIndex = 4;

         m_FillingControls = false;
      }

      private void UpdateForm()
      {
         SensorsGroupBox.Enabled = m_Client != null && m_Client.IsConnected;

         DoorWindowGroupBox.Enabled = m_SensorsSettings.DoorWindowEnabled;
         DimmerGroupBox.Enabled = m_SensorsSettings.DimmerEnabled;
         DistanceGroupBox.Enabled = m_SensorsSettings.DistanceEnabled;
         DustGroupBox.Enabled = m_SensorsSettings.DustEnabled;
         GasGroupBox.Enabled = m_SensorsSettings.GasEnabled;
         HumidityGroupBox.Enabled = m_SensorsSettings.HumidityEnabled;
         LightGroupBox.Enabled = m_SensorsSettings.LightEnabled;
         MotionGroupBox.Enabled = m_SensorsSettings.MotionEnabled;
         PressureGroupBox.Enabled = m_SensorsSettings.PressureEnabled;
         RainMoistureGroupBox.Enabled = m_SensorsSettings.RainMoistureEnabled;
         TemperatureGroupBox.Enabled = m_SensorsSettings.TemperatureEnabled;
         UVGroupBox.Enabled = m_SensorsSettings.UVEnabled;
      }

      private void DoorWindowsCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.DoorWindowEnabled = DoorWindowsCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void DimmerCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.DimmerEnabled = DimmerCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void DistanceCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.DistanceEnabled = DistanceCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void DustCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.DustEnabled = DustCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void GasCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.GasEnabled = GasCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void HumidityCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.HumidityEnabled = HumidityCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void LightCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.LightEnabled = LightCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void MotionCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.MotionEnabled = MotionCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void PressureCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.PressureEnabled = PressureCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void RainMoistureCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.RainMoistureEnabled = RainMoistureCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void TemperatureCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.TemperatureEnabled = TemperatureCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void UVCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         m_SensorsSettings.UVEnabled = UVCheckBox.Checked;
         UpdateDynamicSerializationAttributes();
         UpdateForm();
      }

      private void DoorWindowOpenRadioButton_CheckedChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.DoorWindow = DoorWindowOpenRadioButton.Checked;

            m_FillingControls = false;
         }
      }

      private void DimmerTrackBar_ValueChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.Dimmer = DimmerTrackBar.Value;

            m_FillingControls = false;
         }
      }

      private void DistanceTextbox_TextChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.Distance = int.Parse(DistanceTextbox.Text);

            m_FillingControls = false;
         }
      }

      private void DustTextbox_TextChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.Dust = int.Parse(DustTextbox.Text);

            m_FillingControls = false;
         }
      }

      private void LPGTextbox_TextChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.GasLPG = int.Parse(LPGTextbox.Text);

            m_FillingControls = false;
         }
      }

      private void CO2Textbox_TextChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.GasCO2 = int.Parse(CO2Textbox.Text);

            m_FillingControls = false;
         }
      }

      private void SmokeTextbox_TextChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.GasSmoke = int.Parse(SmokeTextbox.Text);

            m_FillingControls = false;
         }
      }

      private void HumidityTextbox_TextChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.Humidity = float.Parse(HumidityTextbox.Text);

            m_FillingControls = false;
         }
      }

      private void LightTextbox_TextChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.Light = int.Parse(LightTextbox.Text);

            m_FillingControls = false;
         }
      }

      private void MotionTrippedCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.Motion = MotionTrippedCheckBox.Checked;

            m_FillingControls = false;
         }
      }

      private void PressureTextbox_TextChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.Pressure = float.Parse(PressureTextbox.Text);

            m_FillingControls = false;
         }
      }

      private void RainMoistureDetectedCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.RainMoisture = RainMoistureDetectedCheckBox.Checked;

            m_FillingControls = false;
         }
      }

      private void TemperatureTextbox_TextChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.Temperature = float.Parse(TemperatureTextbox.Text);

            m_FillingControls = false;
         }
      }

      private void UVComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (!m_FillingControls)
         {
            m_FillingControls = true;

            m_Sensors.UVIndex = UVComboBox.SelectedIndex;

            m_FillingControls = false;
         }
      }

      private void JSONRadioButton_CheckedChanged(object sender, EventArgs e)
      {
         m_TextFormat = TextFormat.JSON;
      }

      private void XMLRadioButton_CheckedChanged(object sender, EventArgs e)
      {
         m_TextFormat = TextFormat.XML;
      }

      private void PlainTextRadioButton_CheckedChanged(object sender, EventArgs e)
      {
         m_TextFormat = TextFormat.Plain;
      }

      private void IndividualRadioButton_CheckedChanged(object sender, EventArgs e)
      {
         m_TextFormat = TextFormat.Individual;
      }

      private void IntervalComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         m_Interval = int.Parse(IntervalComboBox.SelectedItem.ToString());
      }

      private void BufferContentChangedHandler()
      {
         var sb = new StringBuilder();

         for (var i = 0; i < m_Messages.Count; i++)
         {
            var e = m_Messages[i];
            string value = Utilities.GetString(e.Message);
            sb.Insert(0, String.Format("{0}  -  {1}\r\n", e.Topic, value));
         }

         SetText(sb.ToString());
      }

      private delegate void SubscriptionRichTextBoxInvoker(String text);
      private void SetText(String text)
      {
         if (SubscriptionRichTextBox.InvokeRequired)
            SubscriptionRichTextBox.BeginInvoke(new SubscriptionRichTextBoxInvoker(SetText), text);
         else
            SubscriptionRichTextBox.Text = text;
      }

      private void AsyncSenderThread()
      {
         while (!m_Cancel.IsCancellationRequested)
         {
            if (m_Client != null && m_Client.IsConnected)
            {
               string data;

               switch (m_TextFormat)
               {
                  case TextFormat.JSON:
                     data = Utilities.ToJSON(m_Sensors, m_DynamicContractResolver);
                     break;
                  case TextFormat.XML:
                     data = Utilities.ToXML(m_Sensors, m_XmlAttributeOverrides);
                     break;
                  case TextFormat.Plain:
                  case TextFormat.Individual:
                     data = GetPlainText();
                     break;
                  default:
                     return;
               }

               if (m_Client != null && m_Client.IsConnected)
               {
                  if (m_TextFormat == TextFormat.Individual)
                  {
                     var labels = data.Split(' ');
                     foreach (var label in labels)
                     {
                        if (label.Equals(""))
                           continue;

                        var parts = label.Split('=');
                        string topic = m_Settings.Topic + "/" + parts[0].ToLower();
                        m_Client.Publish(topic, Utilities.GetBytes(parts[1]), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                     }
                  }
                  else
                  {
                     m_Client.Publish(m_Settings.Topic, Utilities.GetBytes(data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                  }
               }

               Thread.Sleep(m_Interval);
            }
         }
      }

      private string GetPlainText()
      {
         var sb = new StringBuilder();

         foreach (var propertieName in m_PropertiesToSend)
         {
            switch (propertieName)
            {
               case Sensors.DOOR_WINDOW :
                  if (m_SensorsSettings.DoorWindowEnabled)
                     sb.Append(Sensors.DOOR_WINDOW + "=" + m_Sensors.DoorWindow + " ");
                  break;
               case Sensors.DIMMER:
                  if (m_SensorsSettings.DimmerEnabled)
                     sb.Append(Sensors.DIMMER + "=" + m_Sensors.Dimmer + " ");
                  break;
               case Sensors.DISTANCE:
                  if (m_SensorsSettings.DistanceEnabled)
                     sb.Append(Sensors.DISTANCE + "=" + m_Sensors.Distance + " ");
                  break;
               case Sensors.DUST:
                  if (m_SensorsSettings.DustEnabled)
                     sb.Append(Sensors.DUST + "=" + m_Sensors.Dust + " ");
                  break;
               case Sensors.GAS_LPG:
                  if (m_SensorsSettings.GasEnabled)
                     sb.Append(Sensors.GAS_LPG + "=" + m_Sensors.GasLPG + " ");
                  break;
               case Sensors.GAS_CO2:
                  if (m_SensorsSettings.GasEnabled)
                     sb.Append(Sensors.GAS_CO2 + "=" + m_Sensors.GasCO2 + " ");
                  break;
               case Sensors.GAS_SMOKE:
                  if (m_SensorsSettings.GasEnabled)
                     sb.Append(Sensors.GAS_SMOKE + "=" + m_Sensors.GasSmoke + " ");
                  break;
               case Sensors.HUMIDITY:
                  if (m_SensorsSettings.HumidityEnabled)
                     sb.Append(Sensors.HUMIDITY + "=" + m_Sensors.Humidity + " ");
                  break;
               case Sensors.LIGHT:
                  if (m_SensorsSettings.LightEnabled)
                     sb.Append(Sensors.LIGHT + "=" + m_Sensors.Light + " ");
                  break;
               case Sensors.MOTION:
                  if (m_SensorsSettings.MotionEnabled)
                     sb.Append(Sensors.MOTION + "=" + m_Sensors.Motion + " ");
                  break;
               case Sensors.PRESSURE:
                  if (m_SensorsSettings.PressureEnabled)
                     sb.Append(Sensors.PRESSURE + "=" + m_Sensors.Pressure + " ");
                  break;
               case Sensors.RAIN_MOISTURE:
                  if (m_SensorsSettings.RainMoistureEnabled)
                     sb.Append(Sensors.RAIN_MOISTURE + "=" + m_Sensors.RainMoisture + " ");
                  break;
               case Sensors.TEMPERATURE:
                  if (m_SensorsSettings.TemperatureEnabled)
                     sb.Append(Sensors.TEMPERATURE + "=" + m_Sensors.Temperature + " ");
                  break;
               case Sensors.UV:
                  if (m_SensorsSettings.UVEnabled)
                     sb.Append(Sensors.UV + "=" + m_Sensors.UVIndex + " ");
                  break;
            }
         }

         return sb.ToString();
      }

      private void Form1_FormClosing(object sender, FormClosingEventArgs e)
      {
         m_Cancel.Cancel();
         if (m_SenderThread != null)
            m_SenderThread.Join();

         if (m_Client != null && m_Client.IsConnected)
            m_Client.Disconnect();
      }
   }
}
