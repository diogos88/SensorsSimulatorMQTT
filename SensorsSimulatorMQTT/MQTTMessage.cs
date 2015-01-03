using System;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace diogos88.MQTT.SensorsSimulator
{
   public class MQTTMessage
   {
      public DateTime Time { get; set; }
      public MqttMsgPublishEventArgs MQTTEvent { get; set; }

      public MQTTMessage(MqttMsgPublishEventArgs mqttEvent, DateTime time)
      {
         MQTTEvent = mqttEvent;
         Time = time;
      }
   }
}
