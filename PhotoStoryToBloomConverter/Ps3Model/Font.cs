using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class Font
    {
        [XmlAttribute("faceName")]
        public string FaceName;
        [XmlAttribute("width")]
        public int Width;
        [XmlAttribute("height")]
        public int Height;
        [XmlAttribute("weight")]
        public int Weight;
        [XmlAttribute("strikeout")]
        public bool Strikeout;
        [XmlAttribute("italic")]
        public bool Italic;
        [XmlAttribute("underline")]
        public bool Underline;
        [XmlAttribute("charset")]
        public bool Charset;
        [XmlAttribute("clipprecision")]
        public int ClipPrecision;
        [XmlAttribute("escapement")]
        public bool Escapement;
        [XmlAttribute("orientation")]
        public int Orientation;
        [XmlAttribute("outprecision")]
        public int OutPrecision;
        [XmlAttribute("pitchandfamily")]
        public int PitchAndFamily;
        [XmlAttribute("quality")]
        public int Quality;
        [XmlAttribute("color")]
        public int Color;
    }
}