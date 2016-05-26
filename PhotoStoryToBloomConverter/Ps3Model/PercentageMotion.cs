using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class PercentageMotion
    {
        [XmlAttribute("manual")]
        public int Manual;

        [XmlElement("RelativeRect")]
        public Rect[] RelativeRects;

        public Rect InitialRect()
        {
            return Manual == -1 ? RelativeRects[0] : RelativeRects[2];
        }

        public Rect FinalRect()
        {
            return Manual == -1 ? RelativeRects[1] : RelativeRects[3];
        }
    }
}