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

    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class Serializator
    {
        public static void Save<T>(List<T> charList, string file)
        {
            using (var fs = new FileStream(file, FileMode.Create))
            {
                var binFormatter = new BinaryFormatter();
                binFormatter.Serialize(fs, charList);
            }
        }

        public static void Save2<T, T2>(Dictionary<T, T2> dic, string file)
        {
            using (var fs = new FileStream(file, FileMode.Create))
            {
                var binFormatter = new BinaryFormatter();
                binFormatter.Serialize(fs, dic);
            }
        }

        public static List<T> Get<T>(string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = new FileStream(file, FileMode.Open))
            {
                try
                {
                    var formatter = new BinaryFormatter();
                    return (List<T>)formatter.Deserialize(fs);
                }
                catch
                {
                    return default(List<T>);
                }
            }

        }

        public static Dictionary<T, T2> Get2<T, T2>(string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = new FileStream(file, FileMode.Open))
            {
                try
                {
                    var formatter = new BinaryFormatter();
                    return (Dictionary<T, T2>)formatter.Deserialize(fs);
                }
                catch
                {
                    return default(Dictionary<T, T2>);
                }
            }

        }
    }
}
