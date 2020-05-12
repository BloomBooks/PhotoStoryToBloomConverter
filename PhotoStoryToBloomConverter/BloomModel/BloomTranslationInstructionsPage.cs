using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using PhotoStoryToBloomConverter.Utilities;
using SIL.IO;

namespace PhotoStoryToBloomConverter.BloomModel
{
	public class BloomTranslationInstructionsPage : BloomPage
	{
		private const string kDefaultText1 = "Attention Translator(s):\nWe are glad that you want to translate this Bible story into your language.  Please be advised that translating anything from the Bible requires other people checking your translation before you publish it (or print it) for distributing to many other people.  \n\nPlease follow these steps when translating this Bible story:\n1. Discuss the meaning of the story with some other people.  Discuss the meaning of any words which you do not know.\n2. Retell in your language.  Practise saying the whole story in your language to somebody else who knows your language.  Practise saying the meaning of each page in your language before you write down the meaning of the page in your language.\n3. Write your translation for each page.  Read out loud and listen to your own translation for each page.  If it does not sound sweet, revise it.";
		private const string kDefaultText2 = "4. Community review.  Ask some other people who speak your language to read out loud and listen to your translation for the story.  Let them suggest any ways to make it sound better in your language.  Revise your translation.\n5. Accuracy check.  A person trained to do Bible accuracy checking must review and approve your story before you publish it. Ask your Bloom trainer to help you find this person if you don\'t know one. This accuracy checker might ask some other people who speak your language to answer some questions about your story (to make sure they understand the translation properly) and to retell it for him/her.  You might need to make more revisions to your translation in the process of this check.\n6. Proofread again. Carefully read out loud and listen again to your whole story.  Make corrections to your spelling and your words so that the translation sounds sweet.\n7. Publish and share with your community.";
		private const string kDefaultText3 = "Thank you for taking the time to carefully and properly translate this Bible story before publishing!  Do not shortcut the process.  It is important for your translated stories to be clear, meaningful, natural sounding, and accurate to the Bible.";

		private readonly string _audioPathForTitle;

		public static IEnumerable<BloomTranslationInstructionsPage> GetDefaultTranslationInstructionPages()
		{
			yield return new BloomTranslationInstructionsPage(kDefaultText1);
			yield return new BloomTranslationInstructionsPage(kDefaultText2);
			yield return new BloomTranslationInstructionsPage(kDefaultText3);
		}

		public BloomTranslationInstructionsPage(string text, string narrationFilePath = null) : base(null, null, null)
		{
			PageClasses = "bloom-page bloom-noreader bloom-nonprinting screen-only Device16x9Portrait layout-style-Default bloom-monolingual";
			PageLabel = "Translation Instructions";
			DataPageLineage = "d054bb76-dc6b-4452-a4ab-f4b83ffa10cc";
			Text = text;

			if (Program.SpAppOutput && narrationFilePath != null)
			{
				// We use the same recording here which is the title on the cover.
				// We need to clone the audio file so if the user re-records one, it doesn't change both.

				// When we get here, we have the .wav in our path, but the file has already been converted to .mp3.
				var audioExtension = ".mp3";
				narrationFilePath = Path.ChangeExtension(narrationFilePath, "mp3");
				var audioDirectoryPath = Path.GetDirectoryName(narrationFilePath);
				_audioPathForTitle = Path.Combine(audioDirectoryPath, $"{Guid.NewGuid()}{audioExtension}");
				RobustFile.Copy(narrationFilePath, _audioPathForTitle);
			}
		}

		public override Div GetMarginBoxChildDiv()
		{
			var translationGroupDiv = new Div
			{
				Class = "bloom-translationGroup",
				DefaultLanguages = "*",
				Divs = new List<Div>
				{
					new Div
					{
						Class = "bloom-editable bloom-noAudio bloom-visibility-code-on",
						ContentEditable = "true",
						Lang = "*",
						FormattedText = GetTextAsParagraphs()
					}
				}
			};
			if (Program.SpAppOutput && _audioPathForTitle != null)
				translationGroupDiv.AudioRecordingMode = "Sentence";
			return new Div
			{
				Class = "split-pane-component-inner",
				Divs = new List<Div> { translationGroupDiv }
			};
		}

		private List<Paragraph> GetTextAsParagraphs()
		{
			if (Program.SpAppOutput && _audioPathForTitle != null)
				return Text.Split('\n').Select(p => new Paragraph { Span = GetSpanSetUpForAudio(p) }).ToList();

			return Text.Split('\n').Select(p => new Paragraph{Text = p}).ToList();
		}

		private Span GetSpanSetUpForAudio(string text)
		{
			var span = new Span {ContentText = text, Class = "audio-sentence", Id = Guid.NewGuid().ToString()};
			if (text != null && text.Trim().StartsWith($"{SpAppMetadata.kTitleIdea}1"))
			{
				span.Id = Path.GetFileNameWithoutExtension(_audioPathForTitle);
				span.Duration = AudioHelper.GetDuration(_audioPathForTitle);
			}
			return span;
		}
	}
}
