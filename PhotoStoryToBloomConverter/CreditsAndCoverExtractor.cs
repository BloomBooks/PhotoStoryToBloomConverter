using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

		private readonly string[] _creditImageHashes = { "3CC9B920651BC4726EED4FFD6FBBDCDA", "8F526256E864725E12F9E31447359504" };
		private readonly string[] _coverImageHashes = { "8C7B5AADFF9AB8B4649481421EB8479F", "781ED3E63E6BD138D9BE59A24EFF7D6A" };

		private readonly string[] _oldCreditImageHashes = { "CDF13EC119AD0128E1196DB518B64BF8", "A3BE4B0D87926E00EDD6884096C80A0C",
			"3A042BB61CBC8FBD841A67B5CC240545", "53956450F833C2B0CB4E4A8AC6432669", "63C8A5355F5287E74ABA26DC9AB377C0" };

		public bool IsCreditsOrCoverPage { get; private set; }
		public string CreditString { get; private set; }
		public ImageIP ImageCopyrightAndLicense { get; private set; }

		//We are assuming that if an image is checked, it is part of the current book, and the credits should be extracted
		public void Extract(string bookName, string imagePath)
		{
			IsCreditsOrCoverPage = true;
			var md5Hash = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(imagePath))).Replace("-", "");
			Debug.WriteLine(md5Hash);

			//Currently all of the information from the credit page needs to be stored in the additional acknowledgments section

			if (string.Equals(md5Hash, _creditImageHashes[0]))
			{
				AddToOrUpdateCreditSlideMap(md5Hash, imagePath);

				CreditString = @"Original illustrations by Jim Padgett, © Sweet Publishing licensed under the terms of a
Creative Commons Attribution-ShareAlike 3.0 Unported license.
www.sweetpublishing.com

Wycliffe Bible Translators, Inc. has skin darkened all of the Jim Padgett illustrations
in our collection, and has modified some of them.


Story script © 2018 Wycliffe Bible Translators, Inc. licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.


Template © 2017 Wycliffe Bible Translators, Inc. licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.


Music © 2017 Wycliffe Bible Translators, Inc. licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.


A special thanks to the 50+ unnamed people who worked on the story scripts, templates, adapted illustrations and music.";

				ImageCopyrightAndLicense = ImageIP.SweetPublishing;
				return;
			}

			if (string.Equals(md5Hash, _creditImageHashes[1]))
			{
				AddToOrUpdateCreditSlideMap(md5Hash, imagePath);

				CreditString = @"Original illustrations by Jim Padgett, © Sweet Publishing licensed under the terms of a
Creative Commons Attribution-ShareAlike 3.0 Unported license.
www.sweetpublishing.com

Wycliffe Bible Translators, Inc. has skin darkened all of the Jim Padgett illustrations
in our collection, and has modified some of them.

Illustrations by Carolyn Dyk © 2001 Wycliffe Bible Translators, Inc. licensed under a Creative Commons Attribution-NonCommercial- NoDerivatives 4.0 International License.


Story script © 2018 Wycliffe Bible Translators, Inc. licensed under the terms of a
Creative Commons Attribution-ShareAlike 4.0 International license.


Template © 2017 Wycliffe Bible Translators, Inc. licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.


Music © 2017 Wycliffe Bible Translators, Inc. licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.


A special thanks to the 50+ unnamed people who worked on the story scripts, templates, adapted illustrations and music.";

				ImageCopyrightAndLicense = ImageIP.SweetPublishingAndWycliffe;
				return;
			}

			if (_oldCreditImageHashes.Contains(md5Hash))
			{
				// This is an old one which we don't expect to see in production.
				AddToOrUpdateCreditSlideMap(md5Hash, imagePath);
				CreditString = "This is just a demo. We need to add real credits.";
				ImageCopyrightAndLicense = ImageIP.Unknown;
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
