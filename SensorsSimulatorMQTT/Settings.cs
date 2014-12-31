using System;

namespace diogos88.MQTT.SensorsSimulator
{
   [Serializable]
   public class Settings
   {
      public string BrokerIP { get; set; } 
      public int Port { get; set; }
      public string Username { get; set; }
      public string Password { get; set; }
      public string Topic { get; set; }

      public Settings()
      {
         BrokerIP = "localhost";
         Port = 1883;
         Username = "";
         Password = "";
         Topic = "#";
      }
   }
}
