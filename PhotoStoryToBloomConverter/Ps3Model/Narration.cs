using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    //Narration is the main voiceover during an image
    public class Narration
    {
        [XmlAttribute("path")]
        public string Path;
    }
}