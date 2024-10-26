using System.Text;
using System.Xml;

namespace ProtocalTool
{
    internal class GeneratorCSharp
    {
        private StringBuilder sb;

        public GeneratorCSharp()
        {
            sb = new StringBuilder();
        }
        public void Generate(string path, string outputPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            using (FileStream fileStream = File.OpenRead(path))
            {
                xmlDocument.Load(fileStream);
            }

            XmlNode root = xmlDocument.SelectSingleNode("messages");
            XmlNodeList enumNodeList = root.SelectNodes("enum");
            GenerateEnums(enumNodeList, outputPath);
            XmlNodeList dataNodeList = root.SelectNodes("data");
            GenerateData(dataNodeList, outputPath);
            XmlNodeList msgNodeList = root.SelectNodes("message");
            GenerateMsg(msgNodeList, outputPath);
        }

        private void GenerateEnums(XmlNodeList enumNodeList, string outputPath)
        {
            string namespaceStr = null;
            string enumNameStr = null;
            string fieldsStr = null;

            foreach (XmlNode enumNode in enumNodeList)
            {
                namespaceStr = enumNode.Attributes["namespace"].InnerText;
                enumNameStr = enumNode.Attributes["name"].InnerText;

                sb.Clear();
                XmlNodeList fieldList = enumNode.SelectNodes("field");
                foreach (XmlNode fieldNode in fieldList)
                {
                    string fieldName = fieldNode.Attributes["name"].InnerText;
                    sb.Append($"\t\t{fieldName}");
                    if (!string.IsNullOrEmpty(fieldNode.InnerText))
                    {
                        sb.Append($" = {fieldNode.InnerText}");
                    }
                    sb.AppendLine(",");
                }
                fieldsStr = sb.ToString();
                sb.Clear();

                sb.Clear();
                sb.AppendLine($"namespace {namespaceStr}");
                sb.AppendLine("{");
                sb.AppendLine($"\tpublic enum {enumNameStr}");
                sb.AppendLine("\t{");
                sb.Append(fieldsStr);
                sb.AppendLine("\t}");
                sb.AppendLine("}");

                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                File.WriteAllText(Path.Combine(outputPath, enumNameStr + ".cs"), sb.ToString());
                sb.Clear();
            }
        }

        private void GenerateData(XmlNodeList nodeList, string outputPath)
        {
            string namespaceStr = null;
            string nameStr = null;
            string fieldsStr = null;
            string getByteCountStr = null;
            string writeStr = null;
            string readStr = null;

            foreach (XmlNode node in nodeList)
            {
                XmlNodeList fieldList = node.SelectNodes("field");

                namespaceStr = node.Attributes["namespace"].InnerText;
                nameStr = node.Attributes["name"].InnerText;
                fieldsStr = DataFields(fieldList, sb);
                getByteCountStr = DataGetByteCount(fieldList, sb);
                writeStr = DataWrite(fieldList, sb);
                readStr = DataRead(fieldList, sb);

                sb.Clear();
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using Common;");
                sb.AppendLine();
                sb.AppendLine($"namespace {namespaceStr}");
                sb.AppendLine("{");
                sb.AppendLine($"\tpublic class {nameStr} : IData");
                sb.AppendLine("\t{");
                sb.Append(fieldsStr);
                sb.AppendLine();
                sb.AppendLine(getByteCountStr);
                sb.AppendLine();
                sb.AppendLine(writeStr);
                sb.AppendLine();
                sb.AppendLine(readStr);
                sb.AppendLine("\t}");
                sb.AppendLine("}");

                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                File.WriteAllText(Path.Combine(outputPath, nameStr + ".cs"), sb.ToString());
                sb.Clear();
            }
        }

        private void GenerateMsg(XmlNodeList nodeList, string outputPath)
        {
            string namespaceStr = null;
            string nameStr = null;
            string idStr = null;
            string fieldsStr = null;
            string getByteCountStr = null;
            string writeStr = null;
            string readStr = null;

            foreach (XmlNode node in nodeList)
            {
                XmlNodeList fieldList = node.SelectNodes("field");

                namespaceStr = node.Attributes["namespace"].InnerText;
                nameStr = node.Attributes["name"].InnerText;
                idStr = node.Attributes["id"].InnerText;
                fieldsStr = DataFields(fieldList, sb);
                getByteCountStr = DataGetByteCount(fieldList, sb);
                writeStr = DataWrite(fieldList, sb);
                readStr = DataRead(fieldList, sb);

                sb.Clear();
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using Common;");
                sb.AppendLine();
                sb.AppendLine($"namespace {namespaceStr}");
                sb.AppendLine("{");
                sb.AppendLine($"\tpublic class {nameStr} : BaseMessage");
                sb.AppendLine("\t{");
                sb.AppendLine($"\t\tpublic override int MegId => {idStr};");
                sb.Append(fieldsStr);
                sb.AppendLine();
                sb.AppendLine(getByteCountStr);
                sb.AppendLine();
                sb.AppendLine(writeStr);
                sb.AppendLine();
                sb.AppendLine(readStr);
                sb.AppendLine("\t}");
                sb.AppendLine("}");

                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                File.WriteAllText(Path.Combine(outputPath, nameStr + ".cs"), sb.ToString());
                sb.Clear();
            }
        }

        private static string GetFieldType(XmlNode xmlNode)
        {
            string fieldTypeStr = xmlNode.Attributes["type"].InnerText;
            if (fieldTypeStr == "list")
            {
                string t = xmlNode.Attributes["T"].InnerText;
                return $"List<{t}>";
            }
            else if (fieldTypeStr == "dic")
            {
                string key = xmlNode.Attributes["TKey"].InnerText;
                string value = xmlNode.Attributes["TValue"].InnerText;
                return $"Dictionary<{key}, {value}>";
            }
            else if (fieldTypeStr == "array")
            {
                string t = xmlNode.Attributes["T"].InnerText;
                return $"{t}[]";
            }
            else if (fieldTypeStr == "enum")
            {
                string t = xmlNode.Attributes["T"].InnerText;
                return t;
            }

            return fieldTypeStr;
        }

        private static string DataFields(XmlNodeList nodeList, StringBuilder sb)
        {
            sb.Clear();
            foreach (XmlNode fieldNode in nodeList)
            {
                string fieldType = GetFieldType(fieldNode);
                string fieldName = fieldNode.Attributes["name"].InnerText;
                sb.AppendLine($"\t\tpublic {fieldType} {fieldName};");
            }
            return sb.ToString();
        }

        private static string DataGetByteCount(XmlNodeList nodeList, StringBuilder sb)
        {
            sb.Clear();
            sb.AppendLine("\t\tpublic override int GetByteCount()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tint num = 0;");

            foreach (XmlNode fieldNode in nodeList)
            {
                string fieldType = fieldNode.Attributes["type"].InnerText;
                string fieldName = fieldNode.Attributes["name"].InnerText;
                if (fieldType == "list")
                {
                    string t = fieldNode.Attributes["T"].InnerText;
                    sb.AppendLine("\t\t\tnum += 4;");
                    sb.AppendLine($"\t\t\tforeach (var item in {fieldName})");
                    sb.AppendLine($"\t\t\t\tnum += {GetTypeBytes(t, "item")};");
                }
                else if (fieldType == "dic")
                {
                    string key = fieldNode.Attributes["TKey"].InnerText;
                    string value = fieldNode.Attributes["TValue"].InnerText;
                    sb.AppendLine("\t\t\tnum += 4;");
                    sb.AppendLine($"\t\t\tforeach (var pair in {fieldName})");
                    sb.AppendLine($"\t\t\t\tnum += {GetTypeBytes(key, "pair.Key")} + {GetTypeBytes(value, "pair.Value")};");
                }
                else if (fieldType == "array")
                {
                    string t = fieldNode.Attributes["T"].InnerText;
                    sb.AppendLine("\t\t\tnum += 4;");
                    sb.AppendLine($"\t\t\tforeach (var item in {fieldName})");
                    sb.AppendLine($"\t\t\t\tnum += {GetTypeBytes(t, "item")};");
                }
                else
                {
                    sb.AppendLine($"\t\t\tnum += {GetTypeBytes(fieldType, fieldName)};");
                }
            }

            sb.AppendLine("\t\t\treturn num;");
            sb.AppendLine("\t\t}");

            return sb.ToString();
        }

        private static string GetTypeBytes(string type, string fieldName)
        {
            switch (type)
            {
                case "int":
                case "float":
                case "enum":
                    return "4";
                case "string":
                    return $"Encoding.UTF8.GetByteCount({fieldName})";
                default:
                    return $"{fieldName}.GetByteCount()";
            }
        }

        private static string DataWrite(XmlNodeList nodeList, StringBuilder sb)
        {
            sb.Clear();
            sb.AppendLine("\t\tpublic override void Write(byte[] bytes, ref int index)");
            sb.AppendLine("\t\t{");

            foreach (XmlNode fieldNode in nodeList)
            {
                string fieldType = fieldNode.Attributes["type"].InnerText;
                string fieldName = fieldNode.Attributes["name"].InnerText;
                if (fieldType == "list")
                {
                    string t = fieldNode.Attributes["T"].InnerText;
                    sb.AppendLine($"\t\t\tWriteInt(bytes, ref index, {fieldName}.Count);");
                    sb.AppendLine($"\t\t\tforeach (var item in {fieldName})");
                    sb.AppendLine($"\t\t\t\t{GetTypeWrite(t, "item")}");
                }
                else if (fieldType == "dic")
                {
                    string key = fieldNode.Attributes["TKey"].InnerText;
                    string value = fieldNode.Attributes["TValue"].InnerText;
                    sb.AppendLine($"\t\t\tWriteInt(bytes, ref index, {fieldName}.Count);");
                    sb.AppendLine($"\t\t\tforeach (var pair in {fieldName})");
                    sb.AppendLine("\t\t\t{");
                    sb.AppendLine($"\t\t\t\t{GetTypeWrite(key, "pair.Key")}");
                    sb.AppendLine($"\t\t\t\t{GetTypeWrite(value, "pair.Value")}");
                    sb.AppendLine("\t\t\t}");
                }
                else if (fieldType == "array")
                {
                    string t = fieldNode.Attributes["T"].InnerText;
                    sb.AppendLine($"\t\t\tWriteInt(bytes, ref index, {fieldName}.Length);");
                    sb.AppendLine($"\t\t\tforeach (var item in {fieldName})");
                    sb.AppendLine($"\t\t\t\t{GetTypeWrite(t, "item")}");
                }
                else
                {
                    sb.AppendLine($"\t\t\t{GetTypeWrite(fieldType, fieldName)}");
                }
            }

            sb.AppendLine("\t\t}");

            return sb.ToString();
        }

        private static string GetTypeWrite(string type, string fieldName)
        {
            switch (type)
            {
                case "int": return $"WriteInt(bytes, ref index, {fieldName});";
                case "float": return $"WriteFloat(bytes, ref index, {fieldName});";
                case "enum": return $"WriteInt(bytes, ref index, (int){fieldName});";
                case "string": return $"WriteString(bytes, ref index, {fieldName});";
                default: return $"WriteIData(bytes, ref index, {fieldName});";
            }
        }

        private static string GetTypeRead(string type)
        {
            switch (type)
            {
                case "int": return $"ReadInt(bytes, ref index)";
                case "float": return $" ReadFloat(bytes, ref index)";
                case "string": return $"ReadString(bytes, ref index)";
                default: return $"ReadIData<{type}>(bytes, ref index)";
            }
        }

        private static string DataRead(XmlNodeList nodeList, StringBuilder sb)
        {
            sb.Clear();
            sb.AppendLine("\t\tpublic override void Read(byte[] bytes, ref int index)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tint temp = 0;");
            foreach (XmlNode fieldNode in nodeList)
            {
                string fieldType = fieldNode.Attributes["type"].InnerText;
                string fieldName = fieldNode.Attributes["name"].InnerText;
                if (fieldType == "list")
                {
                    string t = fieldNode.Attributes["T"].InnerText;
                    sb.AppendLine($"\t\t\t{fieldName} = new List<{t}>();");
                    sb.AppendLine($"\t\t\ttemp = ReadInt(bytes, ref index);");
                    sb.AppendLine("\t\t\tfor (int i = 0; i < temp; i++)");
                    sb.AppendLine($"\t\t\t\t{fieldName}.Add({GetTypeRead(t)});");
                }
                else if (fieldType == "dic")
                {
                    string key = fieldNode.Attributes["TKey"].InnerText;
                    string value = fieldNode.Attributes["TValue"].InnerText;
                    sb.AppendLine($"\t\t\t{fieldName} = new Dictionary<{key}, {value}>();");
                    sb.AppendLine($"\t\t\ttemp = ReadInt(bytes, ref index);");
                    sb.AppendLine("\t\t\tfor (int i = 0; i < temp; i++)");
                    sb.AppendLine($"\t\t\t\t{fieldName}.Add({GetTypeRead(key)}, {GetTypeRead(value)});");
                }
                else if (fieldType == "array")
                {
                    string t = fieldNode.Attributes["T"].InnerText;
                    sb.AppendLine($"\t\t\ttemp = ReadInt(bytes, ref index);");
                    sb.AppendLine($"\t\t\t{fieldName} = new {t}[temp];");
                    sb.AppendLine("\t\t\tfor (int i = 0; i < temp; i++)");
                    sb.AppendLine($"\t\t\t\t{fieldName}[i] = {GetTypeRead(t)};");
                }
                else if (fieldType == "enum")
                {
                    string t = fieldNode.Attributes["T"].InnerText;
                    sb.AppendLine($"\t\t\t{fieldName} = ({t})ReadInt(bytes, ref index);");
                }
                else
                {
                    sb.AppendLine($"\t\t\t{fieldName} = {GetTypeRead(fieldType)};");
                }
            }

            sb.AppendLine("\t\t}");

            return sb.ToString();
        }
    }
}
