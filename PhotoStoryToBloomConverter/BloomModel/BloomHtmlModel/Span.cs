using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Span
    {
        [XmlAttribute("id")]
        public string Id;
        [XmlAttribute("class")]
        public string Class;
        [XmlAttribute("recordingmd5")]
        public string RecordingMD5;
		[XmlAttribute("data-duration")]
	    public string Duration;

        [XmlText]
        public string ContentText;
    }
}