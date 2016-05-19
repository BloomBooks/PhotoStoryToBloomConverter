using System.Xml;
using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    [XmlRoot("html", Namespace= "http://www.w3.org/1999/xhtml", IsNullable = false)]
    public class Html
    {
        public Html()
        {
            Namespaces = new XmlSerializerNamespaces(new [] { new XmlQualifiedName(string.Empty, "http://www.w3.org/1999/xhtml") });
        }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces { get; }

        [XmlElement("head")]
        public Head Head;
        [XmlElement("body")]
        public Body Body;
    }
}