using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.XWPF.UserModel;

namespace PhotoStoryToBloomConverter
{
	class TextExtractor
	{
		private const int kRowLabelColumnIndex = 0;
		private const int kTextColumnIndex = 2;
		private const int kReferenceColumnIndex = 3;

		public static bool TryExtractText(string path, out IList<SourceText> extractedText)
		{
			try
			{
				using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					var doc = new XWPFDocument(file);

					extractedText = new List<SourceText>();

					bool hasAltTitles = false;
					var rows = doc.Tables.SelectMany(t => t.Rows).Skip(1).ToList();
					for (var i = 0; i < rows.Count; i++)
					{
						XWPFTableRow row = rows[i];
						TextType textType = TextType.Text;
						if (i == 0)
						{
							textType = TextType.Title;
							var rowLabel = GetAllTextForCell(row.GetCell(kRowLabelColumnIndex));
							if (rowLabel == "T")
								hasAltTitles = true;
						}
						else if (hasAltTitles && i == 1)
						{
							textType = TextType.AlternateTitlesAndScrRef;
						}

						extractedText.Add(new SourceText
						{
							TextType = textType,
							Text = GetAllTextForCell(row.GetCell(kTextColumnIndex)),
							Reference = GetAllTextForCell(row.GetCell(kReferenceColumnIndex))
						});
					}
				}

				return true;
			}
			catch (Exception e)
			{
				extractedText = null;
				return false;
			}
		}

		private static string GetAllTextForCell(XWPFTableCell cell)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var paragraph in cell.Paragraphs)
			{
				foreach (var run in paragraph.Runs)
					sb.Append(run.Text);

				sb.Append('\n');
			}

			return sb.ToString().Trim();
		}
	}

	public enum TextType
	{
		Title,
		AlternateTitlesAndScrRef,
		Text
	}

	public class SourceText
	{
		public TextType TextType { get; set; }
		public string Text { get; set; }
		public string Reference { get; set; }
	}
}
