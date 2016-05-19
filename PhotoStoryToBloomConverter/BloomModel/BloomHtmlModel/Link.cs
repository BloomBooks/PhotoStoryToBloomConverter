using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Link
    {
        [XmlAttribute("rel")]
        public string Rel;
        [XmlAttribute("href")]
        public string Href;
        [XmlAttribute("type")]
        public string Type;

        public Link()
        {
            
        }

        public Link(string cssHref)
        {
            Rel = "stylesheet";
            Href = cssHref;
            Type = "text/css";
        }
    }
}