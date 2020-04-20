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

		private const string kIntro = "Video Title Slide Content";

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
			var sb = new StringBuilder($"{kIntro}\n" +
				$"Graphic={Graphic.ToDescriptionString()}\n" +
				$"ScriptureReference ={EnsureOneLine(ScriptureReference) }\n" + 
				$"TitleIdeasHeading ={TitleIdeasHeading}"
			);
			for (int i = 0; i < TitleIdeas.Count; i++)
				sb.Append($"\nTitleIdea{i + 1}={TitleIdeas[i]}");
			return sb.ToString();
		}
	}
}
