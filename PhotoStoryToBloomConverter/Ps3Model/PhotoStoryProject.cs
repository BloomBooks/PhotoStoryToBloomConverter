using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    [XmlRoot("MSPhotoStoryProject", Namespace="MSPhotoStory")]
    public class PhotoStoryProject
    {
        [XmlAttribute("schemaVersion")]
        public string SchemaVersion;
        [XmlAttribute("appVersion")]
        public string AppVersion;
        [XmlAttribute("linkOnly")]
        public bool LinkOnly;
        [XmlAttribute("defaultImageDuration")]
        public int DefaultImageDuration;
        [XmlAttribute("visualUnitCount")]
        public int VisualUnitCount;
        [XmlAttribute("codecVersion")]
        public string CodecVersion;
        [XmlAttribute("sessionSeed")]
        public int SessionSeed;
        
        [XmlElement("VisualUnit")]
        public VisualUnit[] VisualUnits;
    }
}