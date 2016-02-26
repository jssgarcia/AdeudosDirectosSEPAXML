﻿using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace XMLSerializerValidator
{
    public static class XMLSerializer
    {
        public static Encoding defaultEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");

        public static string XMLSerializeToString<ObjectType>(ObjectType objetToSerialize, string rootElementName, string defaultNamespace)
        {
            //// Serialize to UTF-8 using UTF8StringWriter()
            //TextWriter stringWriter = new Utf8StringWriter();
            //XMLSerialize<ObjectType>(objetToSerialize, rootElementName, defaultNamespace, stringWriter);
            //return stringWriter.ToString();

            ////Serialize to UTF8 using a MemoryStream
            //var memoryStream = new MemoryStream();
            //var encoding = Encoding.UTF8;
            //TextWriter stringWriter = new StreamWriter(memoryStream, encoding);
            //XMLSerialize<ObjectType>(objetToSerialize, rootElementName, defaultNamespace, stringWriter);
            //return encoding.GetString(memoryStream.ToArray());

            /////Serialize to ISO-8859-1 using a MemoryStream
            //var memoryStream = new MemoryStream();
            //var encoding = Encoding.GetEncoding("ISO-8859-1");
            //TextWriter stringWriter = new StreamWriter(memoryStream, encoding);
            //XMLSerialize<ObjectType>(objetToSerialize, rootElementName, defaultNamespace, stringWriter);
            //return encoding.GetString(memoryStream.ToArray());

            ///Serialize to using a MemoryStream and encoding as parameter
            var memoryStream = new MemoryStream();
            var stringWriter = new StreamWriter(memoryStream, defaultEncoding);
            XMLSerialize<ObjectType>(objetToSerialize, rootElementName, defaultNamespace, stringWriter);
            return defaultEncoding.GetString(memoryStream.ToArray());
        }
        public static string XMLSerializeToString<ObjectType>(ObjectType objetToSerialize, string rootElementName, string defaultNamespace, Encoding encoding)
        {
            var memoryStream = new MemoryStream();
            TextWriter stringWriter = new StreamWriter(memoryStream, encoding);
            XMLSerialize<ObjectType>(objetToSerialize, rootElementName, defaultNamespace, stringWriter);
            return encoding.GetString(memoryStream.ToArray());

            ////Serialize to UTF8 using a MemoryStream
            //var memoryStream = new MemoryStream();
            //var encoding = Encoding.UTF8;
            //TextWriter stringWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            //XMLSerialize<ObjectType>(objetToSerialize, rootElementName, defaultNamespace, stringWriter);
            //return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        public static void XMLSerializeToFile<ObjectType>(ObjectType objetToSerialize, string rootElementName, string defaultNamespace, string targetXMLFileName)
        {
            TextWriter fileWriter = new StreamWriter(targetXMLFileName, true, defaultEncoding);
            XMLSerialize<ObjectType>(objetToSerialize, rootElementName, defaultNamespace, fileWriter);
            fileWriter.Close();
        }

        public static void XMLSerializeToFile<ObjectType>(ObjectType objetToSerialize, string rootElementName, string defaultNamespace, string targetXMLFileName, Encoding encoding)
        {
            TextWriter fileWriter = new StreamWriter(targetXMLFileName, true, encoding);
            XMLSerialize<ObjectType>(objetToSerialize, rootElementName, defaultNamespace, fileWriter);
            fileWriter.Close();
        }

        private static void XMLSerialize<ObjectType>(ObjectType objetToSerialize, string rootElementName, string defaultNamespace, TextWriter xmlWriter)
        {
            XmlSerializer serializer = InitializeSerializer<ObjectType>(rootElementName, defaultNamespace);
            serializer.Serialize(xmlWriter, objetToSerialize);
        }

        public static ObjectType XMLDeserializeFromString<ObjectType>(string xmlString, string rootElementName, string defaultNamespace)
        {
            ObjectType deserializatedObject;
            TextReader stringReader = new StringReader(xmlString);
            deserializatedObject = XMLDeserialize<ObjectType>(stringReader, rootElementName, defaultNamespace);
            stringReader.Close();
            return deserializatedObject;
        }

        public static ObjectType XMLDeserializeFromFile<ObjectType>(string xmlFilePath, string rootElementName, string defaultNamespace)
        {
            ObjectType deserializatedObject;
            TextReader fileReader = new StreamReader(xmlFilePath);
            deserializatedObject = XMLDeserialize<ObjectType>(fileReader, rootElementName, defaultNamespace);
            fileReader.Close();
            return deserializatedObject;
        }

        private static ObjectType XMLDeserialize<ObjectType>(TextReader xmlReader, string rootElementName, string defaultNamespace)
        {
            ObjectType deserializatedObject;
            XmlSerializer serializer = InitializeSerializer<ObjectType>(rootElementName, defaultNamespace);
            deserializatedObject = (ObjectType)serializer.Deserialize(xmlReader);
            return deserializatedObject;
        }

        private static XmlSerializer InitializeSerializer<ObjectType>(string rootElementName, string defaultNamespace)
        {
            XmlSerializer serializer;
            if (rootElementName == null)
            {
                serializer =
                    defaultNamespace == null ?
                    new XmlSerializer(typeof(ObjectType)) :
                    new XmlSerializer(typeof(ObjectType), defaultNamespace);
            }
            else
            {
                serializer =
                    defaultNamespace == null ?
                    new XmlSerializer(typeof(ObjectType), new XmlRootAttribute { ElementName = rootElementName }) :
                    new XmlSerializer(typeof(ObjectType), null, null, new XmlRootAttribute { ElementName = rootElementName }, defaultNamespace);

            }
            return serializer;
        }
    }
}
