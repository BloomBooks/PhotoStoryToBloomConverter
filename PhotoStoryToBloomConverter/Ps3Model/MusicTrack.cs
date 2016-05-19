using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class MusicTrack
    {
        [XmlAttribute("type")]
        public int Type;
        [XmlAttribute("volume")]
        public int Volume;
        [XmlAttribute("colorIndex")]
        public int ColorIndex;

        [XmlElement("SoundTrack")]
        public SoundTrack[] SoundTracks;
    }
}