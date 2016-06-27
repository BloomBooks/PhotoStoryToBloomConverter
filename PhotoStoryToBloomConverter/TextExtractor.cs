using System.Collections.Generic;
using System.IO;
using System.Linq;
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

				text = doc.Tables[0].Rows.Skip(1).Select(row => row.GetCell(2).Paragraphs[0].Runs[0].Text).ToList();
				return true;
			}
			catch
			{
				text = null;
				return false;
			}
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
