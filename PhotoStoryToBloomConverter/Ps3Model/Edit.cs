using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class Edit
    {
        [XmlElement("RotateAndCrop")]
        public RotateAndCrop RotateAndCrop;
        [XmlElement("TextOverlay")]
        public TextOverlay[] TextOverlays;
    }
}