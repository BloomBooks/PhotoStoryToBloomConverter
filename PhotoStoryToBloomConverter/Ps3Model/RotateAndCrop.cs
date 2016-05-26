using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    //Edit done to the original image before using as the base image for a motion
    public class RotateAndCrop
    {
        //Unused in current samples from photostory
        [XmlAttribute("rotateType")]
        public string RotateType;

        [XmlElement("Rectangle")]
        public Rect CroppedRect;
    }
}