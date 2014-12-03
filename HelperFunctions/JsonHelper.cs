using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace HelperFunctions
{
    public class JSONHelper
    {
        /// <summary>
        /// Uses System.Runtime.Serialization
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        public static string ToJSON(object obj, Type objType)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(objType);
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            return Encoding.Default.GetString(ms.ToArray());
        }

        /// <summary>
        /// Uses System.Runtime.Serialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static T FromJSON<T>(string jsonString)
        {
            T obj = Activator.CreateInstance<T>();

            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));

            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());

            obj = (T)serializer.ReadObject(ms);

            ms.Close();
            ms.Dispose();

            return obj;
        }

        /// <summary>
        /// Uses Newtonsoft.Json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        //public static T FromJsonNewtonsoft<T>(string json, IEnumerable<NewtonsoftOptions> newtonsoftOptions = null)
        public static T FromJsonNewtonsoft<T>(string json)
        {
            //if (newtonsoftOptions.IsNullOrEmpty())
            //{
            //    JsonSerializer js = new JsonSerializer();//newtonsoft
            //    using (JsonTextReader jtr = new JsonTextReader(new StringReader(json)))
            //    {
            //        return js.Deserialize<T>(jtr);
            //    }
            //}
            //else
            //{
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

            //foreach (NewtonsoftOptions option in newtonsoftOptions)
            //{
            //    switch (option)
            //    {
            //        case NewtonsoftOptions.IgnoreNullValues:
            //            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            //            break;

            //        default:
            //            break;
            //    }
            //}

            return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
            //}
        }

        /// <summary>
        /// Uses Newtonsoft.Json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJsonNewtonsoft<T>(string json, bool ignoreNulValues)
        {
            if (!ignoreNulValues)
            {
                JsonSerializer js = new JsonSerializer();//newtonsoft
                using (JsonTextReader jtr = new JsonTextReader(new StringReader(json)))
                {
                    return js.Deserialize<T>(jtr);
                }
            }
            else
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;

                return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
            }
        }

        /// <summary>
        /// Uses Newtonsoft.Json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string ToJsonNewtonsoft(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Get JSON string from raw string
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static string GetCorrectJSONString(string row)
        {
            StringBuilder sb = new StringBuilder();

            int bracketCounter = 0;
            for (int i = 0; i < row.Length; i++)
            {
                sb.Append(row[i]);//first we add char because it can be not bracket

                if (row[i] != '{' && row[i] != '}') continue;

                if (row[i] == '{')
                    bracketCounter++;
                else
                    bracketCounter--;

                if (bracketCounter == 0)
                {
                    return sb.ToString();
                }
            }

            sb = null;
            return String.Empty;
        }


        public static void LogJson(string path, string json)
        {
            if (path.IsNullOrWhiteSpace()) return;

            if (json.IsNullOrWhiteSpace())
            {
                File.WriteAllText(path, "This json is NULL or WhiteSpace");
            }
            else
            {
                File.WriteAllText(path, json);
            }
        }

        public static bool IsJson(string jsonText)
        {
            if (jsonText.IsNullOrWhiteSpace())
                return false;
            else if (jsonText.StartsWith("{") && jsonText.EndsWith("}"))
            {
                return true;
            }

            return false;
        }
    }
}
