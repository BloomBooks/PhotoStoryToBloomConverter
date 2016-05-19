using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using PhotoStoryToBloomConverter.PS3Model;

namespace PhotoStoryToBloomConverter
{
    public static class Ps3AndBloomSerializer
    {
        private static XmlSerializer Ps3ProjectSerializer()
        {
            var serializer = new XmlSerializer(typeof(PhotoStoryProject));
            serializer.UnknownNode += UnknownNodeLogger;
            serializer.UnknownAttribute += UnknownAttributeLogger;
            return serializer;
        }
        private static XmlSerializer BloomHtmlSerializer()
        {
            var serializer = new XmlSerializer(typeof(Html), new XmlRootAttribute("html"));
            serializer.UnknownNode += UnknownNodeLogger;
            serializer.UnknownAttribute += UnknownAttributeLogger;
            return serializer;
        }

        public static PhotoStoryProject DeserializePhotoStoryXml(string filename)
        {
            var xmlFileStream = new FileStream(filename, FileMode.Open);
            return (PhotoStoryProject)Ps3ProjectSerializer().Deserialize(xmlFileStream);
        }

        public static Html DeserializeBloomHtml(string filename)
        {
            var xmlFileStream = new FileStream(filename, FileMode.Open);
            return (Html)BloomHtmlSerializer().Deserialize(xmlFileStream);
        }

        public static void SerializeBloomHtml(Html html, string destinationFilename)
        {
            var xmlFileStream = new FileStream(destinationFilename, FileMode.Create);
            var writer = XmlWriter.Create(xmlFileStream, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            var xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);
            BloomHtmlSerializer().Serialize(writer, html, xns);
        }

        public static string SerializeBloomHtml(Html html)
        {
            var stringBuilder = new StringBuilder();
            var writer = XmlWriter.Create(stringBuilder, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            var xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);
            BloomHtmlSerializer().Serialize(writer, html, xns);
            return stringBuilder.ToString();
        }

        private static void UnknownNodeLogger(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private static void UnknownAttributeLogger(object sender, XmlAttributeEventArgs e)
        {
            var attr = e.Attr;
            Console.WriteLine("Unknown attribute " +
            attr.Name + "='" + attr.Value + "'");
        }
    }
}