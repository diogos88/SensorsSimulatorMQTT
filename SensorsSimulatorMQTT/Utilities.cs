using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace diogos88.MQTT.SensorsSimulator
{
   public class Utilities
   {
      public static bool Serialize<T>(T obj, string path)
      {
         try
         {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StreamWriter(path))
            {
               serializer.Serialize(writer, obj);
            }

            return true;
         }
         catch
         {
            return false;
         }
      }

      public static T Deserialize<T>(string path)
      {
         var xmlData = default(T);

         try
         {
            var deserializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(path))
            {
               object obj = deserializer.Deserialize(reader);
               xmlData = (T)obj;
            }

            return xmlData;
         }
         catch
         {
            //
         }

         return xmlData;
      }

      public static T Clone<T>(T obj)
      {
         if (!typeof(T).IsSerializable)
            throw new ArgumentException(@"The type must be serializable.", "obj");

         if (ReferenceEquals(obj, null))
            return default(T);

         var formatter = new BinaryFormatter();
         var stream = new MemoryStream();

         using (stream)
         {
            formatter.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
         }
      }

      public static bool CompareComplexObjects<T>(T obj1, T obj2)
      {
         if (obj1 == null || obj2 == null)
            return false;

         var obj1Str = ToXML(obj1);
         var obj2Str = ToXML(obj2);
         return obj1Str.Equals(obj2Str);
      }

      public static string ToXML<T>(T obj, XmlAttributeOverrides xmlAttOverrides = null)
      {
         var serializer = (xmlAttOverrides == null) ? new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T), xmlAttOverrides);

         using (var writer = new StringWriter())
         {
            serializer.Serialize(writer, obj);
            return writer.ToString();
         }
      }

      public static string ToJSON<T>(T obj, DynamicContractResolver contractResolver = null)
      {
         if (obj == null)
            return "";

         if (contractResolver == null)
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
         else
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = contractResolver });
      }

      public static byte[] GetBytes(string str)
      {
         //var bytes = new byte[str.Length * sizeof(char)];
         //Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
         //return bytes;

         return System.Text.Encoding.UTF8.GetBytes(str);
      }

      public static string GetString(byte[] data)
      {
         //var chars = new char[data.Length / sizeof(char)];
         //Buffer.BlockCopy(data, 0, chars, 0, data.Length);
         //return new string(chars);

         return System.Text.Encoding.UTF8.GetString(data);
      }

      public static List<string> GetConstants(Type type)
      {
         var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
         return fieldInfos.Select(fieldinfo => fieldinfo.GetRawConstantValue().ToString()).ToList();
      }
   }
}
