using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using PhotoStoryToBloomConverter.Utilities;

namespace PhotoStoryToBloomConverter
{
	public enum SpAppMetadataGraphic
	{
		[Description("gray-background")]
		GrayBackground,
		[Description("front-cover-graphic")]
		FrontCoverGraphic
	}

	public class SpAppMetadata
	{
		public SpAppMetadataGraphic Graphic { get; set; }
		public string ScriptureReference { get; set; }
		public string TitleIdeasHeading { get; set; }
		public List<string> TitleIdeas { get; }

		private const string kIntro = "CONTENT FOR THE TITLE SLIDE (slide #0) in SP APP";
		private const string kInstructions = @"INSTRUCTIONS:
After ""Graphic="" type either ""gray-background"" or ""front-cover-picture"" in English to indicate your choice for the title slide image.
After ""ScriptureReference="" type a Scripture reference or a subtitle for your story in the LWC.
After ""TitleIdeasHeading="" type something like ""Ideas for the story title:"" in the LWC.
After ""TitleIdea1="" type a sample title in the LWC. (Always complete this line providing a title example.)
After ""TitleIdea2="" type another sample title in the LWC. (Or leave this line blank.)
After ""TitleIdea3="" type another sample title in the LWC. (Or leave this line blank.)";

		public SpAppMetadata(string scriptureReference, string titleIdeasHeading, List<string> titleIdeas)
		{
			ScriptureReference = scriptureReference;
			TitleIdeasHeading = titleIdeasHeading;
			TitleIdeas = titleIdeas;
		}

		public string EnsureOneLine(string possibleMultiLine)
		{
			return string.Join("; ", possibleMultiLine.Split('\n'));
		}

		public override string ToString()
		{
			var sb = new StringBuilder($"{kIntro}\n\n" +
				$"Graphic={Graphic.ToDescriptionString()}\n" +
				$"ScriptureReference ={EnsureOneLine(ScriptureReference) }\n" + 
				$"TitleIdeasHeading ={TitleIdeasHeading}"
			);
			for (int i = 0; i < TitleIdeas.Count; i++)
				sb.Append($"\nTitleIdea{i + 1}={TitleIdeas[i]}");
			sb.Append("\n\n");
			sb.Append(kInstructions);
			return sb.ToString();
		}
	}
}
