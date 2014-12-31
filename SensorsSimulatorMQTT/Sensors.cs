using System;

namespace diogos88.MQTT.SensorsSimulator
{
   [Serializable]
   public class Sensors
   {
      /// <summary>
      /// Door / Windows / Button
      /// Range: LOW-HIGH
      /// </summary>
      public bool DoorWindow { get; set; }
      /// <summary>
      /// Dimmer
      /// Range: 0-100
      /// </summary>
      public int Dimmer { get; set; }
      /// <summary>
      /// Distance sensor
      /// Range: 2-500cm
      /// </summary>
      public int Distance { get; set; }
      /// <summary>
      /// Dust sensor
      /// Range: 0 to 1023 (density?)
      /// </summary>
      public int Dust { get; set; }
      /// <summary>
      /// Gas sensor (Liquefied petroleum gas)
      /// ppm
      /// </summary>
      public int GasLPG { get; set; }
      /// <summary>
      /// Gas sensor (Carbon Dioxide)
      /// ppm
      /// </summary>
      public int GasCO2 { get; set; }
      /// <summary>
      /// Gas sensor (Smoke)
      /// ppm
      /// </summary>
      public int GasSmoke { get; set; }
      /// <summary>
      /// Humidity sensor
      /// DHT-11: 20%-80%RH
      /// DHT-22: 0%-100%RH
      /// </summary>
      public float Humidity { get; set; }
      /// <summary>
      /// Light sensor
      /// Range: 0-1024 (lux?)
      /// </summary>
      public int Light { get; set; }
      /// <summary>
      /// Motion sensor
      /// Range: LOW-HIGH
      /// </summary>
      public bool Motion { get; set; }
      /// <summary>
      /// Pressure sensor
      /// Pa
      /// </summary>
      public float Pressure { get; set; }
      /// <summary>
      /// Soil moisture sensor
      /// Range: LOW-HIGH
      /// </summary>
      public bool RainMoisture { get; set; }
      /// <summary>
      /// Temperature sensor
      /// Celcius
      /// </summary>
      public float Temperature { get; set; }
      /// <summary>
      /// UV sensor
      /// Range: 0-12
      /// </summary>
      public int UV { get; set; }

      public Sensors()
      {
         DoorWindow = false;
         Dimmer = 50;
         Distance = 150;
         Dust = 0;
         GasLPG = 0;
         GasCO2 = 0;
         GasSmoke = 0;
         Humidity = 50.0f;
         Light = 100;
         Motion = false;
         Pressure = 100;
         RainMoisture = true;
         Temperature = 20.0f;
         UV = 0;
      }
   }
}
