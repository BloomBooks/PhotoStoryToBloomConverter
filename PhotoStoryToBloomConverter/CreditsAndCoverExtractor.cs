using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PhotoStoryToBloomConverter
{
	class CreditsAndCoverExtractor
	{
		public enum ImageIP
		{
			Unknown,
			SweetPublishing,
			SweetPublishingAndWycliffe
		}

		private static readonly Dictionary<string, List<string>> s_creditSlideMap = new Dictionary<string, List<string>>();

		private readonly string[] _creditImageHashes = { "A3BE4B0D87926E00EDD6884096C80A0C", "3A042BB61CBC8FBD841A67B5CC240545",
			"53956450F833C2B0CB4E4A8AC6432669", "63C8A5355F5287E74ABA26DC9AB377C0" };
		private readonly string[] _coverImageHashes = { "8C7B5AADFF9AB8B4649481421EB8479F", "781ED3E63E6BD138D9BE59A24EFF7D6A" };

		public bool IsCreditsOrCoverPage { get; private set; }
		public string CreditString { get; private set; }
		public ImageIP ImageCopyrightAndLicense { get; private set; }

		//We are assuming that if an image is checked, it is part of the current book, and the credits should be extracted
		public void Extract(string bookName, string imagePath)
		{
			IsCreditsOrCoverPage = true;
			var md5Hash = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(imagePath))).Replace("-", "");
			//Currently all of the information from the credit page needs to be stored in the additional acknowledgments section

			if (string.Equals(md5Hash, _creditImageHashes[0]))
			{
				AddToOrUpdateCreditSlideMap(md5Hash, imagePath);
				// 001 In the Beginning
				/* This story is © Sweet Publishing. This story was adapted by VM Productions from the
				 * READ-N-GROW PICTURE BIBLE and used with permission.
				 * It may be adapted for use in your language.
				 * -------------------------------------------
				 * Template & Music © 2016 SIL
				 * You may use the template and include music for creating Bible Story Videos.
				 * -------------------------------------------
				 * Illustrations by Jim Padgett, Courtesy of Sweet Publishing, Ft. Worth, TX. © 1984
				 * Illustrations skin-darkened and adapted by SIL - VM Productions
				 * TERMS OF USE FOR READ-N-GROW ILLUSTRATIONS
				 * These illustrations are © Sweet Publishing and are made available under the terms of a
				 * Creative Commons Attribution-Share Alike 4.0 International license. [license image]
				 * -------------------------------------------
				 * TERMS OF USE FOR Carolyn Dyk Illustrations, with filenames ending in 'CD'
				 * This work is licensed under a Creative Commons
				 * Attribution-NonCommercial-NoDerivatives 4.0 International License. [license image]
				 * You may crop and resize but not modify the images for your new work.
				 * Images may be rotated or flipped horizontally, provided this does not contradict
				 * historical fact or violate cultural norms.
				 * Illustrations by Carolyn Dyk, © 2001 Wycliffe Bible Translators, Inc. Used with permission. */
				CreditString = @"READ-N-GROW illustrations courtesy of Sweet Publishing, Ft. Worth, TX. Illustrated by Jim Padgett  © 1984 Sweet Publishing CC-BY-SA 4.0.
SIL - VM Productions adapted this story and skin-darkened illustrations from the READ-N-GROW PICTURE BIBLE. Animation template & music © 2016 SIL [Hopefully creative commons license here]
-------------------------------------------
TERMS OF USE FOR Carolyn Dyk Illustrations, with filenames ending in 'CD'
This work is licensed under a Creative Commons
Attribution-NonCommercial-NoDerivatives 4.0 International License.
You may crop and resize but not modify the images for your new work.
Images may be rotated or flipped horizontally, provided this does not contradict historical fact or violate cultural norms.
Illustrations by Carolyn Dyk, © 2001 Wycliffe Bible Translators, Inc. Used with permission.";

				ImageCopyrightAndLicense = ImageIP.SweetPublishingAndWycliffe;
				return;
			}
			if (string.Equals(md5Hash, _creditImageHashes[1]) || string.Equals(md5Hash, _creditImageHashes[2]) || string.Equals(md5Hash, _creditImageHashes[3]))
			{
				AddToOrUpdateCreditSlideMap(md5Hash, imagePath);
				// 003, 046, 078
				/* This story is © Sweet Publishing. This story was adapted by VM Productions from the
				 * READ-N-GROW PICTURE BIBLE and used by permission.
				 * It may be adapted for use in your language.
				 * -------------------------------------------
				 * Template & Music © 2016 SIL
				 * You may use the template and include music for creating Bible Story Videos
				 * -------------------------------------------
				 * Illustrations by Jim Padgett, Courtesy of Sweet Publishing, Ft. Worth, TX. © 1984
				 * Illustrations skin-darkened and adapted by SIL - VM Productions
				 * TERMS OF USE FOR READ-N-GROW ILLUSTRATIONS
				 * These illustrations are © Sweet Publishing and are made available under the terms of a
				 * Creative Commons Attribution-Share Alike 4.0 International license. [license image] */
				CreditString = @"READ-N-GROW illustrations courtesy of Sweet Publishing, Ft. Worth, TX. Illustrated by Jim Padgett © 1984 Sweet Publishing CC-BY-SA 4.0.
SIL - VM Productions adapted this story and skin-darkened illustrations from the READ-N-GROW PICTURE BIBLE. Animation template & music © 2016 SIL [Hopefully creative commons license here]";

				ImageCopyrightAndLicense = ImageIP.SweetPublishing;
				return;
			}

			foreach (var coverImageHash in _coverImageHashes)
			{
				if (string.Equals(md5Hash, coverImageHash))
				{
					AddToOrUpdateCreditSlideMap(md5Hash, imagePath);
					return;
				}
			}
			IsCreditsOrCoverPage = false;
		}

		private void AddToOrUpdateCreditSlideMap(string md5Hash, string imagePath)
		{
			List<string> slideList;
			if (s_creditSlideMap.TryGetValue(md5Hash, out slideList))
			{
				if (slideList == null)
					slideList = new List<string>();
				slideList.Add(imagePath);
			}
			else
			{
				s_creditSlideMap.Add(md5Hash, new List<string> {imagePath});
			}
		}

		public static void CreateMapFile()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var pair in s_creditSlideMap)
			{
				foreach (var list in pair.Value)
					sb.Append(pair.Key).Append(" ").Append(list).Append(Environment.NewLine);
			}
			File.WriteAllText("mapFile.txt", sb.ToString());
		}
	}
}
