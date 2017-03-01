using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Span
	{
		private string _id;

		[XmlAttribute("id")]
	    public string Id
	    {
			get
			{
				// We don't want to serialize to invalid HTML
				return string.IsNullOrWhiteSpace(_id) ? null : _id;
			}
		    set { _id = value; }
	    }

	    [XmlAttribute("class")]
        public string Class;
		[XmlAttribute("data-duration")]
	    public string Duration;

        [XmlText]
        public string ContentText;
    }
}