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
		[Description("front-cover-picture")]
		FrontCoverGraphic
	}

	public class SpAppMetadata
	{
		public const string kTitleIdea = "TitleIdea";
		public SpAppMetadataGraphic Graphic { get; set; }
		public string ScriptureReference { get; }
		public string TitleIdeasHeading { get; }
		public List<string> TitleIdeas { get; }

		private const string kIntro = "CONTENT FOR THE TITLE SLIDE (slide #0) in SP APP";
		private const string kInstructions = @"INSTRUCTIONS:
After ""Graphic="" type either ""gray-background"" or ""front-cover-picture"" in English to indicate your choice for the title slide image.
After ""ScriptureReference="" type a Scripture reference or a subtitle for your story in the LWC.
After ""TitleIdeasHeading="" type something like ""Ideas for the story title:"" in the LWC.
After ""TitleIdea1="" type a sample title in the LWC. (Always complete this line providing a title example.)
After ""TitleIdea2="" (3, etc) type another sample title in the LWC. (Or leave this line blank.)";

		public SpAppMetadata(string scriptureReference, string titleIdeasHeading, List<string> titleIdeas)
		{
			ScriptureReference = scriptureReference;
			TitleIdeasHeading = titleIdeasHeading;
			TitleIdeas = titleIdeas;
		}

		public override string ToString()
		{
			var sb = new StringBuilder($"{kIntro}\n\n" +
				$"Graphic={Graphic.ToDescriptionString()}\n" +
				$"ScriptureReference={ScriptureReference}\n" + 
				$"TitleIdeasHeading={TitleIdeasHeading}"
			);
			for (int i = 0; i < TitleIdeas.Count; i++)
				sb.Append($"\n{kTitleIdea}{i + 1}={TitleIdeas[i]}");
			sb.Append("\n\n");
			sb.Append(kInstructions);
			return sb.ToString();
		}
	}
}
