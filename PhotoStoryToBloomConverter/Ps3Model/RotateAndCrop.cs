using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class RotateAndCrop
    {
        [XmlAttribute("rotateType")]
        public string RotateType;

        [XmlElement("Rectangle")]
        public Rect Rect;
    }
}