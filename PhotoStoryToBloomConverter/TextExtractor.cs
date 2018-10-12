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
		public static bool TryExtractText(string path, out IList<string> text)
		{
			try
			{
				XWPFDocument doc;
				using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
					doc = new XWPFDocument(file);

				text = doc.Tables[0].Rows.Skip(1).Select(row => GetAllTextForCell(row.GetCell(2))).ToList();

				// The templates usually have a copyright slide last with "*" as the text (sometimes it just starts with "*")
				// The newest templates have a blank slide just before that for a "pause".
				int elementToRemoveCount = 0;
				foreach (var textElement in text.Reverse())
				{
					if (string.IsNullOrWhiteSpace(textElement) || textElement.StartsWith("*"))
						elementToRemoveCount++;
					else
						break;
				}
				for (int i = 0; i < elementToRemoveCount; i++)
					text.RemoveAt(text.Count - 1);
				return true;
			}
			catch (Exception e)
			{
				text = null;
				return false;
			}
		}

		private static string GetAllTextForCell(XWPFTableCell cell)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var run in cell.Paragraphs.SelectMany(paragraph => paragraph.Runs))
				sb.Append(run.Text);
			return sb.ToString().Trim();
		}

		private string GetReference(XWPFTableRow row)
		{
			try
			{
				return row.GetCell(3).Paragraphs[0].Runs[0].Text;
			}
			catch
			{
				return null;
			}
		}
	}
}
