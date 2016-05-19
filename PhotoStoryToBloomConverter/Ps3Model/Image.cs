using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class Ps3Image
    {
        [XmlAttribute("path")]
        public string Path;
        [XmlAttribute("comments")]
        public string Comments;
        [XmlAttribute("lastModified")]
        public string LastModified;
        [XmlAttribute("width")]
        public int Width;
        [XmlAttribute("height")]
        public int Height;
        [XmlAttribute("noNarration")]
        public int NoNarration;
        [XmlAttribute("useManualDuration")]
        public int UseManualDuration;
        [XmlAttribute("narrationTips")]
        public string NarrationTips;

        [XmlElement("Edit")]
        public Edit[] Edits;
        [XmlElement("Motion")]
        public Motion[] Motions;
        [XmlElement("Motion2")]
        public Motion[] Motions2;
        [XmlElement("Transition2")]
        public Transition[] Transitions;
        [XmlElement("MusicTrack")]
        public MusicTrack[] MusicTracks;
    }
}