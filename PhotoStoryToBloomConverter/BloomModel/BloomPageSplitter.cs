using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoStoryToBloomConverter.BloomModel
{
    public class BloomPageSplitter
    {
	    public IList<KeyValuePair<Language, string>> Text { get; set; }
	    public BloomImage Image { get; set; }
	    public BloomAudio Audio { get; set; }

	    public Div ConvertToHtml()
        {
            var imageDiv = Image.ConvertToHtml();
            return new Div
            {
                Class = "split-pane horizontal-percent",
                Style = "min-height: 42px;",
                Divs = new List<Div>
                {
                    new Div
                    {
                        Class = "split-pane-component position-top",
                        Style = "bottom: 50%;",
                        Divs = new List<Div>
                        {
                            new Div
                            {
                                Class = "split-pane-component-inner",
                                Style = "position: relative;",
                                MinHeight = "60px 150px 250px",
                                MinWidth = "60px 150px 250px",
                                Divs = new List<Div> { imageDiv }
                            }
                        }
                    },
                    new Div
                    {
                        Class = "split-pane-divider horizontal-divider",
                        Style = "bottom: 50%;",
                        Title = "50%",
                        SimpleText = ""
                    },
                    new Div
                    {
                        Class = "split-pane-component position-bottom",
                        Style = "height: 50%;",
                        Divs = new List<Div>
                        {
                            new Div 
                            {
                                Class = "split-pane-component-inner adding",
                                Style = "position: relative;",
                                MinWidth = "60px 150px 250px",
                                Divs = GetContentDivs()
                            }
                        }
                    }
                }
            };
        }

	    private Span GetNarrationSpan(string text)
	    {
			var contentText = string.IsNullOrWhiteSpace(text) ? "nbsp;" : text;

		    return Audio.NarrationPath == null ?
				new Span { ContentText = contentText } :
			    new Span
			    {
				    Class = "audio-sentence", Id = Path.GetFileNameWithoutExtension(Audio.NarrationPath),
				    ContentText = contentText, Duration = Audio.Duration
			    };
	    }

	    private List<Div> GetContentDivs()
	    {
			var divs = new List<Div>();
		    if (Text.Select(kv => kv.Key).All(k => k != Language.English))
		    {
			    var dummyEnglishText = new List<KeyValuePair<Language, string>> { new KeyValuePair<Language, string>(Language.English, "nbsp;") };
			    divs.AddRange(new List<Div>
			    {
				    new Div
				    {
					    Class = "box-header-off bloom-translationGroup",
					    Divs = GetBoxHeaderOffDivs(dummyEnglishText)
				    },
				    new Div
				    {
					    Class = "bloom-translationGroup bloom-trailingElement normal-style",
					    Divs = GetTrailingElementDivs(dummyEnglishText)
				    }
			    });
		    }
			divs.AddRange(new List<Div>
			{
				new Div
				{
					Class = "box-header-off bloom-translationGroup",
					Divs = GetBoxHeaderOffDivs(Text)
				},
				new Div
				{
					Class = "bloom-translationGroup bloom-trailingElement normal-style",
					Divs = GetTrailingElementDivs(Text)
				}
			});
		    return divs;
	    } 

	    private List<Div> GetBoxHeaderOffDivs(IList<KeyValuePair<Language, string>> text)
	    {
			var list = new List<Div>();
		    foreach (var kv in text)
		    {
			    var language = kv.Key;
			    list.Add(new Div
			    {
				    Class = "bloom-editable cke_editablecke_editable_inline cke_contents_ltr" + (language == Language.English ? " bloom-content1" : ""),
				    ContentEditable = "true",
				    Lang = language.GetCode(),
				    TabIndex = "0",
				    Spellcheck = "true",
				    Role = "textbox",
				    AriaLabel = "false",
				    FormattedText = new List<Paragraph> { new Paragraph { Text = "" }}
			    });
		    }
		    return list;
	    }

		private List<Div> GetTrailingElementDivs(IList<KeyValuePair<Language, string>> text)
		{
			var list = new List<Div>();
		    foreach (var kv in text)
		    {
			    var language = kv.Key;
			    var narrationSpan = GetNarrationSpan(kv.Value);
			    list.Add(new Div
			    {
				    Class = "bloom-editable cke_editable cke_editable_inline cke_contents_ltr normal-style" + (language == Language.English ? " bloom-content1" : ""),
				    ContentEditable = "true",
				    Lang = language.GetCode(),
				    Style = "min-height: 24px;",
				    TabIndex = "0",
				    Spellcheck = "true",
				    Role = "textbox",
				    AriaLabel = "false",
				    DataLanguageTipContent = language.ToString(),
				    FormattedText = new List<Paragraph> { new Paragraph { Span = narrationSpan }}
			    });
		    }
			return list;
		}
    }
}
