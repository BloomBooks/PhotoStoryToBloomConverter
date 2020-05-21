using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Paragraph
    {
        [XmlElement("span")]
        public Span Span;

        [XmlText]
        public string Text;

        public static Paragraph GetParagraphForTextWithAudio(string text, string audioFilePath)
        {
	        if (String.IsNullOrWhiteSpace(audioFilePath))
		        return new Paragraph {Text = text};
	        return new Paragraph
	        {
		        Span = new Span {Id = Path.GetFileNameWithoutExtension(audioFilePath), Class = "audio-sentence", ContentText = text}
	        };
        }

        public static List<Paragraph> GetMultiParagraphFromString(string input)
        {
	        var result = new List<Paragraph>();
	        foreach (var paragraph in input.Split(new[] { "\r\n" }, StringSplitOptions.None))
		        result.Add(new Paragraph {Text = paragraph});
	        return result;
        }

        public static List<Paragraph> GetFormattedText(string text)
        {
	        return new List<Paragraph> {new Paragraph {Text = text}};
        }
    }
}
