using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Div
    {
        [XmlAttribute("id")]
        public string Id;
        [XmlAttribute("class")]
        public string Class;
        [XmlAttribute("lang")]
        public string Lang;
        [XmlAttribute("style")]
        public string Style;


        [XmlAttribute("role")]
        public string Role;
        [XmlAttribute("spellcheck")]
        public string Spellcheck;
        [XmlAttribute("tabindex")]
        public string TabIndex;

        [XmlAttribute("min-height")]
        public string MinHeight;
        [XmlAttribute("min-width")]
        public string MinWidth;
        [XmlAttribute("title")]
        public string Title;
        [XmlAttribute("contenteditable")]
        public string ContentEditable;

        [XmlAttribute("data-page")]
        public string DataPage;
        [XmlAttribute("data-export")]
        public string DataExport;
        [XmlAttribute("data-pagelineage")]
        public string DataPageLineage;
        [XmlAttribute("data-book")]
        public string DataBook;
        [XmlAttribute("data-copyright")]
        public string DataCopyright;
        [XmlAttribute("data-creator")]
        public string DataCreator;
        [XmlAttribute("data-license")]
        public string DataLicense;
        [XmlAttribute("data-hint")]
        public string DataHint;
        [XmlAttribute("data-metalanguage")]
        public string DataMetaLanguage;
        [XmlAttribute("data-functiononhintclick")]
        public string DataFunctionOnHintClick;
        [XmlAttribute("data-derived")]
        public string DataDerived;
        [XmlAttribute("data-library")]
        public string DataLibrary;
        [XmlAttribute("data-hasqtip")]
        public string DataHasQtip;
        [XmlAttribute("data-haslanguagetipcontent")]
        public string DataHasLanguageTipContent;
        [XmlAttribute("data-languagetipcontent")]
        public string DataLanguageTipContent;
        [XmlAttribute("data-initialrect")]
        public string DataInitialRect;
        [XmlAttribute("data-finalrect")]
        public string DataFinalRect;
        [XmlAttribute("data-duration")] 
        public string DataDuration;
        [XmlAttribute("data-i18n")]
        public string DataI18n;

        [XmlAttribute("aria-describedby")]
        public string AriaDescribedBy;
        [XmlAttribute("aria-label")]
        public string AriaLabel;

        [XmlAttribute("data-backgroundaudio")]
        [DefaultValue("")]
        public string BackgroundAudio;

        [XmlAttribute("data-backgroundaudiovolume")]
        public string BackgroundAudioVolume;

        [XmlText]
        public string SimpleText;

        [XmlElement("p")]
        public Paragraph FormattedText;
        [XmlElement("div")]
        public List<Div> Divs;
        [XmlElement("label")]
        public Label[] Labels;
        [XmlElement("span")]
        public Span[] Spans;
        [XmlElement("img")]
        public Img[] Imgs;
    }
}