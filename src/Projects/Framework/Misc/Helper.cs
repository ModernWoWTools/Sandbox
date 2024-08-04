using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Framework.Constants.Misc;
using Framework.Logging;

namespace Framework.Misc
{
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: all", Exclude = true, ApplyToMembers = false)]
    public class Helper
    {
        static IFormatProvider enUSNumber = new CultureInfo("en-US").NumberFormat;
        public static void PrintHeader(string serverName)
        {
            Log.Message(LogType.Init, @"_____________World of Warcraft___________");
            Log.Message(LogType.Init, @"                   _   _                 ");
            Log.Message(LogType.Init, @"    /\            | | (_)                ");
            Log.Message(LogType.Init, @"   /  \   _ __ ___| |_ _ _   _ _ __ ___  ");
            Log.Message(LogType.Init, @"  / /\ \ | '__/ __| __| | | | | '_ ` _ \ ");
            Log.Message(LogType.Init, @" / ____ \| | | (__| |_| | |_| | | | | | |");
            Log.Message(LogType.Init, @"/_/    \_\_|  \___|\__|_|\__,_|_| |_| |_|");
            Log.Message(LogType.Init, "");

            var sb = new StringBuilder();

            sb.Append("_________________________________________");

            var nameStart = (42 - serverName.Length) / 2;

            sb.Insert(nameStart, serverName);
            sb.Remove(nameStart + serverName.Length, serverName.Length);

            Log.Message(LogType.Init, sb.ToString());
            Log.Message(LogType.Init, $"{"www.arctium.io",27}");

            Log.Message();
            Log.Message(LogType.Normal, $"Starting Arctium {serverName}...");
            Log.Message();
        }

        public static float ParseFloat(string data)
        {
            return float.Parse(data, enUSNumber);
        }

        public static uint GetCurrentUnixTimestampMillis()
        {
            return (uint)DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public static uint GetUnixTime()
        {
            var baseDate = new DateTime(1970, 1, 1);
            var currentDate = DateTime.Now;
            var ts = currentDate - baseDate;

            return (uint)ts.TotalSeconds;
        }

        public static uint GetUnixTime2()
        {
            var baseDate = new DateTime(1970, 1, 1);
            var currentDate = DateTime.Now;
            var ts = currentDate - baseDate;

            return (uint)ts.Milliseconds;
        }

        public static string DataDirectory()
        {
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            var ret = Assembly.GetExecutingAssembly().Location.Replace(name + ".exe", "").Replace(name + ".dll", "");

            return ret;
        }


    }

    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: all", Exclude = true, ApplyToMembers =false)]
    public class HttpHeader
    {
        // GET or POST
        public string Method { get; set; }
        // URL
        public string Path { get; set; }
        // HTTP/1.1
        public string Type { get; set; }
        // IP Address
        public string Host { get; set; }
        // enUS, deDE, ...
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
        public string AcceptLanguage { get; set; }
        public string Accept { get; set; }
        public string UserAgent { get; set; }
        //public string Date { get; set; }

        public string Content { get; set; }
    }

    public enum HttpCode
    {
        OK = 200,
    }

    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: all", Exclude = true, ApplyToMembers = false)]
    public class HttpResponse
    {
        public static byte[] Create(HttpCode httpCode, HttpHeader header)
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                sw.WriteLine($"HTTP/1.1 {(int)httpCode} {httpCode.ToString()}");

                var date = DateTime.Now.ToUniversalTime().ToString("r");

                sw.WriteLine($"Date: {date}");
                sw.WriteLine("Server: Arctium");
                sw.WriteLine("Retry-After: 600");
                sw.WriteLine($"Content-Length: {header.ContentLength}");
                sw.WriteLine("Vary: Accept-Encoding");
                sw.WriteLine($"Content-Type: application/json;charset=UTF-8");// {header.ContentType}");

                sw.WriteLine();

                sw.WriteLine(header.Content);
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }

    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: all", Exclude = true, ApplyToMembers = false)]
    public class HttpRequest
    {
        public static HttpHeader Parse(byte[] data)
        {
            var headerValues = new Dictionary<string, object>();
            var header = new HttpHeader();

            using (var sr = new StreamReader(new MemoryStream(data)))
            {
                var info = sr.ReadLine().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                headerValues.Add("method", info[0]);
                headerValues.Add("path", info[1]);
                headerValues.Add("type", info[2]);

                while (!sr.EndOfStream)
                {
                    info = sr.ReadLine().Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                    if (info.Length == 2)
                        headerValues.Add(info[0].Replace("-", "").ToLower(), info[1]);
                    else if (info.Length > 2)
                    {
                        var val = "";

                        info.Skip(1).Select(s => val += s);

                        headerValues.Add(info[0].Replace("-", "").ToLower(), val);
                    }
                    else
                    {
                        // We are at content here.
                        var content = sr.ReadLine();

                        headerValues.Add("content", content);
                    }
                }
            }

            var httpFields = typeof(HttpHeader).GetProperties();

            foreach (var f in httpFields)
            {
                object val = null;

                if (headerValues.TryGetValue(f.Name.ToLower(), out val))
                {
                    if (f.PropertyType == typeof(int))
                        f.SetValue(header, Convert.ChangeType(Convert.ToInt32(val), f.PropertyType));
                    else
                        f.SetValue(header, Convert.ChangeType(val, f.PropertyType));
                }
            }

            return header;
        }
    }
}
