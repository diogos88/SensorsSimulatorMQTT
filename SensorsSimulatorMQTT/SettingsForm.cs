using System;
using System.Windows.Forms;

namespace diogos88.MQTT.SensorsSimulator
{
   public partial class SettingsForm : Form
   {
      readonly Settings m_Settings;
      readonly Settings m_SettingsCopy;

      public Settings SettingsCopy { get { return m_SettingsCopy; } }

      public SettingsForm(Settings settings)
      {
         InitializeComponent();

         m_Settings = settings;
         m_SettingsCopy = Utilities.Clone(settings);
      }

      private void SettingsForm_Load(object sender, EventArgs e)
      {
         UpdateFields();
      }

      private void UpdateFields()
      {
         BrokerURLTextBox.Text = m_SettingsCopy.BrokerIP;
         PortTextBox.Text = m_SettingsCopy.Port.ToString();
         UsernameTextBox.Text = m_SettingsCopy.Username;
         PasswordTextBox.Text = m_SettingsCopy.Password;
         TopicTextBox.Text = m_SettingsCopy.Topic;
      }

      private void UpdateSettingsCopy()
      {
         m_SettingsCopy.BrokerIP = BrokerURLTextBox.Text;
         m_SettingsCopy.Port = int.Parse(PortTextBox.Text);
         m_SettingsCopy.Username = UsernameTextBox.Text;
         m_SettingsCopy.Password = PasswordTextBox.Text;
         m_SettingsCopy.Topic = TopicTextBox.Text;
      }

      private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
      {
         UpdateSettingsCopy();

         if (DialogResult != System.Windows.Forms.DialogResult.OK)
         {
            if (!Utilities.CompareComplexObjects(m_Settings, m_SettingsCopy))
            {
               e.Cancel = (MessageBox.Show(@"Cancel modifications", @"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No);
            }
         }
      }
   }
}
