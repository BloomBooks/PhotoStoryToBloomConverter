using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    //Music track represents background music, usually triggered by an image
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