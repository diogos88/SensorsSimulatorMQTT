using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace diogos88.MQTT.SensorsSimulator
{
   public class DynamicContractResolver : DefaultContractResolver
   {
      private readonly IList<string> m_PropertiesToSerialize;

      public DynamicContractResolver(IList<string> propertiesToSerialize)
      {
         m_PropertiesToSerialize = propertiesToSerialize;
      }

      protected override IList<JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
      {
         IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
         return properties.Where(p => m_PropertiesToSerialize.Contains(p.PropertyName)).ToList();
      }
   }
}
