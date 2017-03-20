using System;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;

namespace VMUVUnityPlugin_NET35_v100
{
    static class JSONUtilities
    {
        public static string CreatePadCalTerms(DEV2Pad pad)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);
            CalTerms terms = pad.ExportCalTerms();

            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            writer.WritePropertyName("ID");
            writer.WriteValue(terms.id);
            writer.WritePropertyName("Max Value");
            writer.WriteValue(terms.maxValue);
            writer.WritePropertyName("Min Value");
            writer.WriteValue(terms.minValue);
            writer.WritePropertyName("Coordinate x");
            writer.WriteValue(terms.coordinate.x);
            writer.WritePropertyName("Coordinate y");
            writer.WriteValue(terms.coordinate.y);
            writer.WritePropertyName("Coordinate z");
            writer.WriteValue(terms.coordinate.z);
            writer.WritePropertyName("Sensitivity");
            writer.WriteValue(terms.sensitivity);
            writer.WriteEndObject();

            return sb.ToString();
        }

        public static void WriteJsonFile(string path, string jsonFile)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                var calFile = File.CreateText(path);
                calFile.WriteLine(jsonFile);
                calFile.Close();
            }
            catch (Exception e)
            {
                DEV2ExceptionHandler.TakeActionOnException(e);
            }
        }
    }
}
