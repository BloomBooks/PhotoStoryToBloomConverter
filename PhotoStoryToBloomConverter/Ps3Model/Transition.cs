using System;
using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    //Used to indicate how to transition between visual units
    public class Transition
    {
        [XmlAttribute("duration")]
        public float Duration;
        //A 0 means don't use the duration set here, a -1 means use this duration
        [XmlAttribute("useManualDuration")]
        public int UseManualDuration;
        [XmlAttribute("withPrevImage")]
        public int WithPrevImage;
        //Typically an arbitrary hash, usually stands for cross-fade 
        [XmlAttribute("type")]
        public string Type;
    }
}