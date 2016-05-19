using System;
using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    [Serializable]
    public class Transition
    {
        [XmlAttribute("duration")]
        public float Duration;
        [XmlAttribute("useManualDuration")]
        public int UseManualDuration;
        [XmlAttribute("withPrevImage")]
        public int WithPrevImage;
        [XmlAttribute("type")]
        public string Type;
    }
}