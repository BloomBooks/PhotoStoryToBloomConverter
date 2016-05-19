using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class VisualUnit
    {
        [XmlAttribute("duration")]
        public float Duration;
        [XmlAttribute("type")]
        public int Type;

        [XmlElement("Transition")] public Transition[] Transitions;
        [XmlElement("Image")] public Ps3Image Image;
        [XmlElement("Narration")] public Narration Narration;
    }
}