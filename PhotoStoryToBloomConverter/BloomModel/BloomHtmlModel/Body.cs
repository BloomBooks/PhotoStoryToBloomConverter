using System.Collections.Generic;
using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Body
    {
		// REVIEW: I don't think these two are necessary/useful
        [XmlAttribute("bookcreationtype")]
        public string BookCreationType;
        [XmlAttribute("class")]
        public string Class;

		// Features used to make this a "motion book"
		[XmlAttribute("data-bfautoadvance")]
		public string BloomFeature_AutoAdvance = "landscape;bloomReader";
		[XmlAttribute("data-bfcanrotate")]
		public string BloomFeature_CanRotate = "allOrientations;bloomReader";
		[XmlAttribute("data-bfplayanimations")]
		public string BloomFeature_PlayAnimations = "landscape;bloomReader";
		[XmlAttribute("data-bfplaymusic")]
		public string BloomFeature_PlayMusic = "landscape;bloomReader";
		[XmlAttribute("data-bfplaynarration")]
		public string BloomFeature_PlayNarration = "landscape;bloomReader";
		[XmlAttribute("data-bffullscreenpicture")]
		public string BloomFeature_FullScreenPicture = "landscape;bloomReader";

		[XmlElement("div")]
        public List<Div> Divs;
    }
}
