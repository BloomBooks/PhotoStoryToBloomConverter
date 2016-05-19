using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class Narration
    {
        [XmlAttribute("path")]
        public string Path;
    }
}