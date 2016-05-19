using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class SoundTrack
    {
        [XmlAttribute("comments")]
        public string Comments;
        [XmlAttribute("path")]
        public string Path;
    }
}