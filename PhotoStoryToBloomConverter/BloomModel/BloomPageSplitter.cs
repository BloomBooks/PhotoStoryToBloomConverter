using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoStoryToBloomConverter.BloomModel
{
	public class BloomPageSplitter
	{
		public IList<KeyValuePair<Language, SourceText>> Text { get; set; }
		public BloomImage Image { get; set; }
		public BloomAudio Audio { get; set; }

		public Div ConvertToHtml()
		{
			var imageDiv = Image.ConvertToHtml();
			var topPaneContent = new List<Div> {imageDiv};
			List<Div> bottomPaneContent =
				!Program.SpAppOutput ?
					GetContentDivs() :
					new List<Div> {GetHorizontalSplit(12f, GetContentDivs(true), GetContentDivs())};
			return GetHorizontalSplit(41.3f, topPaneContent, bottomPaneContent);
		}

		private Div GetHorizontalSplit(float splitAt, List<Div> topPaneContent, List<Div> bottomPaneContent)
		{
			return new Div
			{
				Class = "split-pane horizontal-percent",
				Style = "min-height: 42px;",
				Divs = new List<Div>
				{
					GetTopPane(splitAt, topPaneContent),
					GetMainHorizontalDivider(splitAt),
					GetBottomPane(splitAt, bottomPaneContent)
				}
			};
		}

		private Div GetTopPane(float percentFromTop, List<Div> content)
		{
			return new Div
			{
				Class = "split-pane-component position-top",
				Style = $"bottom: {100 - percentFromTop}%;",
				Divs = new List<Div> { GetSplitPaneComponentInner(content) }
			};
		}

		private Div GetMainHorizontalDivider(float percentFromTop)
		{
			return new Div
			{
				Class = "split-pane-divider horizontal-divider",
				Style = $"bottom: {100 - percentFromTop}%;",
				Title = $"{percentFromTop}%",
				SimpleText = ""
			};
		}

		private Div GetBottomPane(float percentFromTop, List<Div> content)
		{
			return new Div
			{
				Class = "split-pane-component position-bottom",
				Style = $"height: {100 - percentFromTop}%;",
				Divs = new List<Div> { GetSplitPaneComponentInner(content) }
			};
		}

		private Div GetSplitPaneComponentInner(List<Div> children)
		{
			return new Div
			{
				Class = "split-pane-component-inner",
				Style = "position: relative;",
				MinHeight = "60px 150px 250px",
				MinWidth = "60px 150px 250px",
				Divs = children
			};
		}

		private IEnumerable<Paragraph> GetNarrationParagraphs(string text)
		{
			var contentText = string.IsNullOrWhiteSpace(text) ? " " : text;

			var paragraphs = contentText.Split('\n');

			foreach (var paragraph in paragraphs)
				yield return new Paragraph { Text = paragraph };
		}

		private List<Div> GetContentDivs(bool references = false)
		{
			var divs = new List<Div>();
			if (Text.Select(kv => kv.Key).All(k => k != Language.English))
			{
				var dummyEnglishSourceText = new SourceText{Text = "nbsp;"};
				var dummyEnglishKvps = new List<KeyValuePair<Language, SourceText>> { new KeyValuePair<Language, SourceText>(Language.English, dummyEnglishSourceText) };
				divs.AddRange(new List<Div>
				{
					new Div
					{
						Class = "bloom-translationGroup bloom-trailingElement normal-style",
						Divs = GetDivsInTranslationGroup(dummyEnglishKvps)
					}
				});
			}
			divs.AddRange(new List<Div>
			{
				new Div
				{
					Class = "bloom-translationGroup bloom-trailingElement normal-style",
					Divs = GetDivsInTranslationGroup(Text, references)
				}
			});
			return divs;
		}

		private List<Div> GetDivsInTranslationGroup(IList<KeyValuePair<Language, SourceText>> sourceTextKvps, bool references = false)
		{
			var list = new List<Div>();
			foreach (var kv in sourceTextKvps)
			{
				var language = kv.Key;
				var div = new Div
				{
					Class = "bloom-editable normal-style" + (language == Language.English ? " bloom-content1" : ""),
					ContentEditable = "true",
					Lang = language.GetCode(),
					Style = "min-height: 24px;",
					TabIndex = "0",
					Spellcheck = "true",
					Role = "textbox",
					AriaLabel = "false",
					DataLanguageTipContent = language.ToString(),
					FormattedText = GetNarrationParagraphs(references ? kv.Value.Reference : kv.Value.Text).ToList()
				};

				if (!references && !string.IsNullOrWhiteSpace(Audio.NarrationPath))
				{
					div.Class += " audio-sentence";
					div.Id = Path.GetFileNameWithoutExtension(Audio.NarrationPath);
					div.Duration = Audio.Duration;
					div.AudioRecordingMode = "TextBox";
				}

				list.Add(div);
			}
			return list;
		}
	}
}
