using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Img
    {
        [XmlAttribute("data-creator")]
        public string DataCreator;
        [XmlAttribute("data-license")]
        public string DataLicense;
        [XmlAttribute("data-copyright")]
        public string DataCopyright;
        [XmlAttribute("style")]
        public string Style;
        [XmlAttribute("alt")]
        public string Alt;
        [XmlAttribute("src")]
        public string Src;
        [XmlAttribute("data-derived")]
        public string DataDerived;
        [XmlAttribute("class")]
        public string Class;
        [XmlAttribute("width")]
        public int Width;
        [XmlAttribute("height")]
        public int Height;
    }
}