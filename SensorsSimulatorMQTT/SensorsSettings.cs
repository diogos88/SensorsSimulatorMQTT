namespace diogos88.MQTT.SensorsSimulator
{
   public class SensorsSettings
   {
      public static readonly int[] UV_INDEXES = { 50, 227, 318, 408, 503, 606, 696, 795, 881, 976, 1079, 1170, 3000 };
      public static readonly float[] GAS_LPG_CURVE = { 2.3f, 0.21f, -0.47f };
      public static readonly float[] GAS_COC_CURVE = { 2.3f, 0.72f, -0.34f };
      public static readonly float[] GAS_SMOKE_CURVE = { 2.3f, 0.53f, -0.44f };
      public static readonly int DISTANCE_MAX = 300; // cm

      public bool DoorWindowEnabled { get; set; }
      public bool DimmerEnabled { get; set; }
      public bool DistanceEnabled { get; set; }
      public bool DustEnabled { get; set; }
      public bool GasEnabled { get; set; }
      public bool HumidityEnabled { get; set; }
      public bool LightEnabled { get; set; }
      public bool MotionEnabled { get; set; }
      public bool PressureEnabled { get; set; }
      public bool RainMoistureEnabled { get; set; }
      public bool TemperatureEnabled { get; set; }
      public bool UVEnabled { get; set; }

      public SensorsSettings()
      {
         DoorWindowEnabled = false;
         DimmerEnabled = false;
         DistanceEnabled = false;
         DustEnabled = false;
         GasEnabled = false;
         HumidityEnabled = false;
         LightEnabled = false;
         MotionEnabled = false;
         PressureEnabled = false;
         RainMoistureEnabled = false;
         TemperatureEnabled = false;
         UVEnabled = false;
      }
   }
}
