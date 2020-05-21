using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using PhotoStoryToBloomConverter.Utilities;
using SIL.IO;

namespace PhotoStoryToBloomConverter
{
	public enum SpAppMetadataGraphic
	{
		[Description("gray-background")]
		GrayBackground,
		[Description("front-cover-picture")]
		FrontCoverGraphic
	}

	public class SpAppMetadata
	{
		public const string kTitleIdeaKey = "spTitleIdea";
		public SpAppMetadataGraphic Graphic { get; set; }
		public string ScriptureReference { get; }
		public string TitleIdeasHeading { get; }
		public List<string> TitleIdeas { get; }
		public string TitleIdeaNarrationPath { get; private set; }

		public SpAppMetadata(string scriptureReference, string titleIdeasHeading, List<string> titleIdeas)
		{
			ScriptureReference = scriptureReference;
			TitleIdeasHeading = titleIdeasHeading;
			TitleIdeas = titleIdeas;
		}

		public void PrepareNarrationAudio(string narrationFilePath)
		{
			// We use the same recording here which is the title on the cover.
			// We need to clone the audio file so if the user re-records one, it doesn't change both.

			// When we get here, we have the .wav in our path, but the file has already been converted to .mp3.
			var audioExtension = ".mp3";
			narrationFilePath = Path.ChangeExtension(narrationFilePath, "mp3");
			var audioDirectoryPath = Path.GetDirectoryName(narrationFilePath);
			if (audioDirectoryPath == null)
				return;

			TitleIdeaNarrationPath = Path.Combine(audioDirectoryPath, $"{Guid.NewGuid()}{audioExtension}");
			RobustFile.Copy(narrationFilePath, TitleIdeaNarrationPath);
		}

		public List<Div> GetSpAppMetadataForDataDiv(string language1Code)
		{
			var divs = new List<Div>();
			divs.AddRange(new List<Div>
			{
				new Div
				{
					DataBook = "spGraphic", Lang = "*",
					FormattedText = Paragraph.GetFormattedText(Graphic.ToDescriptionString())
				},
				new Div
				{
					DataBook = "spReference", Lang = language1Code,
					FormattedText = Paragraph.GetFormattedText(ScriptureReference)
				},
				new Div
				{
					DataBook = "spTitleIdeasHeading", Lang = language1Code,
					FormattedText = Paragraph.GetFormattedText(TitleIdeasHeading)
				}
			});
			if (TitleIdeas?.Count > 0)
			{
				for (int i = 0; i < TitleIdeas.Count; i++)
				{
					List<Paragraph> formattedText;
					if (i == 0)
						formattedText = new List<Paragraph> {Paragraph.GetParagraphForTextWithAudio(TitleIdeas[0], TitleIdeaNarrationPath)};
					else
						formattedText = Paragraph.GetFormattedText(TitleIdeas[i]);

					divs.Add(new Div
					{
						DataBook = $"{kTitleIdeaKey}{i + 1}",
						Lang = language1Code,
						FormattedText = formattedText
					});
				}
			}

			return divs;
		}
	}
}
