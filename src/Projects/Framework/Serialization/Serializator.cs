using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Framework.Serialization
{
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class Json
    {
        public static string CreateString<T>(T dataObject)
        {
            return JsonConvert.SerializeObject(dataObject, Formatting.Indented);// Encoding.UTF8.GetString(CreateArray(dataObject));
        }

        public static T CreateObject<T>(string jsonData)
        {
            return JsonConvert.DeserializeObject<T>(jsonData);// CreateObject<T>(Encoding.UTF8.GetBytes(jsonData));
        }
    }
}
