using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class Ps3Image
    {
        [XmlAttribute("path")]
        public string Path;
        //Comments generally indicates the next image name, not always accurate
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
        public AbsoluteMotion AbsoluteMotion;
        [XmlElement("Motion2")]
        public PercentageMotion PercentageMotion;
        [XmlElement("Transition2")]
        public Transition[] Transitions;
        [XmlElement("MusicTrack")]
        public MusicTrack[] MusicTracks;
    }
}