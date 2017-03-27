using System;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;

namespace VMUVUnityPlugin_NET35_v100
{
    static class JSONUtilities
    {
        public static string CreatePadCalTerms(DEV2Pad[] pads)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();

            for (int i = 0; i < pads.Length; i++)
            {
                CalTerms terms = pads[i].ExportCalTerms();
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
            }

            writer.WritePropertyName("DrawRadius");
            writer.WriteValue(CurrentValueTable.GetDrawRadius());
            writer.WritePropertyName("Speed X");
            writer.WriteValue(CurrentValueTable.GetSpeedMultiplier());
            writer.WritePropertyName("Strafe Enabled");
            writer.WriteValue(1);

            writer.WriteEndObject();

            return sb.ToString();
        }

        public static CalTerms[] ReadPadCalTermsFromJson(string json)
        {
            CalTerms[] calTerms = new CalTerms[9];
            JsonTextReader reader = new JsonTextReader(new StringReader(json));

            try
            {
                for (int i = 0; i < calTerms.Length; i++)
                {
                    string val = GetNextJsonValue(reader);
                    calTerms[i].id = UInt16.Parse(val);
                    val = GetNextJsonValue(reader);
                    calTerms[i].maxValue = UInt16.Parse(val);
                    val = GetNextJsonValue(reader);
                    calTerms[i].minValue = UInt16.Parse(val);
                    val = GetNextJsonValue(reader);
                    calTerms[i].coordinate.x = float.Parse(val);
                    val = GetNextJsonValue(reader);
                    calTerms[i].coordinate.y = float.Parse(val);
                    val = GetNextJsonValue(reader);
                    calTerms[i].coordinate.z = float.Parse(val);
                    val = GetNextJsonValue(reader);
                    calTerms[i].sensitivity = float.Parse(val);
                }

                string tmp = GetNextJsonValue(reader);
                CurrentValueTable.SetDrawRadius(float.Parse(tmp));
                tmp = GetNextJsonValue(reader);
                CurrentValueTable.SetSpeedMultiplier(float.Parse(tmp));
                tmp = GetNextJsonValue(reader);
                CurrentValueTable.SetStrafeEnabled(int.Parse(tmp));
            }
            catch(Exception e)
            {
                DEV2ExceptionHandler.TakeActionOnException(e);
            }

            return calTerms;
        }

        public static string GetNextJsonValue(JsonTextReader reader)
        {
            do
            {
                reader.Read();
            } while (reader.Value == null || reader.TokenType == JsonToken.PropertyName);

            return reader.Value.ToString();
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

        public static string ReadJsonFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string calFile = File.ReadAllText(path);
                    return calFile;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                DEV2ExceptionHandler.TakeActionOnException(e);
                return null;
            }
        }
    }
}
