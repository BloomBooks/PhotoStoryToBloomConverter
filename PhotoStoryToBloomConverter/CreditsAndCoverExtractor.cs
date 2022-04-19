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
		public enum CreditsType
		{
			Unknown,
			SweetPublishing,
			SweetPublishingAndWycliffe,
			PaulWhiteAndHolden,
			PaulWhiteNessAndHolden,
			LostCoinRheburg,
			NotCredits
		}

		private static readonly Dictionary<string, List<string>> CreditSlideMap = new Dictionary<string, List<string>>();

		private static readonly Dictionary<string, CreditsType> ImageHashToIpInfoDictionary = new Dictionary<string, CreditsType>
		{
			{ "3CC9B920651BC4726EED4FFD6FBBDCDA", CreditsType.SweetPublishing },
			{ "3A042BB61CBC8FBD841A67B5CC240545", CreditsType.SweetPublishing },
			{ "53956450F833C2B0CB4E4A8AC6432669", CreditsType.SweetPublishing },
			{ "63C8A5355F5287E74ABA26DC9AB377C0", CreditsType.SweetPublishing },
			{ "F043C2C0C21DBB6D3628BAE2A907F09D", CreditsType.SweetPublishing },

			{ "8F526256E864725E12F9E31447359504", CreditsType.SweetPublishingAndWycliffe },
			{ "A3BE4B0D87926E00EDD6884096C80A0C", CreditsType.SweetPublishingAndWycliffe },
			{ "588D1C78C0395E258C503F563CD66A9A", CreditsType.SweetPublishingAndWycliffe },

			{ "108CCAF0758894DD92D41E7B85577F98", CreditsType.SweetPublishing },

			{ "EEC5D3512C67A54E319AFDDDE74BCA1E", CreditsType.SweetPublishingAndWycliffe },

			{ "07C0F906D56881C5605369051943ED9A", CreditsType.PaulWhiteAndHolden },

			{ "F81D6C857A1929C4C8F4FEC5D40440DC", CreditsType.PaulWhiteNessAndHolden },

			{ "FDA950D9E21F2A1DC1FBCBFC1E965E8D", CreditsType.LostCoinRheburg },

			// Blank gray cover images
			{ "8C7B5AADFF9AB8B4649481421EB8479F", CreditsType.NotCredits },
			{ "781ED3E63E6BD138D9BE59A24EFF7D6A", CreditsType.NotCredits },

			// This is an old one which we don't expect to see in production.
			{ "CDF13EC119AD0128E1196DB518B64BF8", CreditsType.Unknown },
		};

		public bool IsCreditsOrCoverPage { get; private set; }
		public string CreditString { get; private set; }
		public CreditsType ImageCopyrightAndLicense { get; private set; }

		//We are assuming that if an image is checked, it is part of the current book, and the credits should be extracted
		public void Extract(string imagePath)
		{
			IsCreditsOrCoverPage = false;
			var md5Hash = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(imagePath))).Replace("-", "");
			Debug.WriteLine(md5Hash);

			if (ImageHashToIpInfoDictionary.Keys.Contains(md5Hash))
			{
				IsCreditsOrCoverPage = true;
				var imageType = ImageHashToIpInfoDictionary[md5Hash];
				switch (imageType)
				{
					case CreditsType.NotCredits:
						AddToOrUpdateCreditSlideMap(md5Hash, imagePath);
						return;
					default:
						AddToOrUpdateCreditSlideMap(md5Hash, imagePath);
						CreditString = GetCreditString(imageType);
						ImageCopyrightAndLicense = imageType;
						return;
				}
			}
		}

		//Currently all of the information from the credit page needs to be stored in the additional acknowledgments section
		public static string GetCreditString(CreditsType imageType)
		{
			switch (imageType)
			{
				case CreditsType.SweetPublishing:
					return @"Original illustrations by Jim Padgett, © Sweet Publishing, licensed under the terms of a Creative Commons Attribution-ShareAlike 3.0 Unported license.
www.sweetpublishing.com
Wycliffe Bible Translators, Inc. has skin darkened all of the Jim Padgett illustrations in our collection, and has modified many of them.

Story script, Template, and Music © 2022 Wycliffe Bible Translators, Inc., licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.

A special thanks to the 50+ unnamed people who worked on the story scripts, templates, adapted illustrations and music.";


				case CreditsType.SweetPublishingAndWycliffe:
					return @"Original illustrations by Jim Padgett, © Sweet Publishing, licensed under the terms of a Creative Commons Attribution-ShareAlike 3.0 Unported license.
www.sweetpublishing.com
Wycliffe Bible Translators, Inc. has skin darkened all of the Jim Padgett illustrations in our collection, and has modified many of them.
Illustrations by Carolyn Dyk © 2001 Wycliffe Bible Translators, Inc. licensed under a Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International license.

Story script, Template, and Music © 2022 Wycliffe Bible Translators, Inc. licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.

A special thanks to the 50+ unnamed people who worked on the story scripts, templates, adapted illustrations and music.";


				case CreditsType.LostCoinRheburg:
					return @"The Lost Coin
Text from Luke 15:8-10
Illustrations by: Judith Rheburg, SIL © Licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.

Story script, Template, and Music © 2022 Wycliffe Bible Translators, Inc., licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.
A special thanks to the unnamed people who worked on the story script, template, and music.";



				case CreditsType.PaulWhiteAndHolden:
					return @"Based on the Jungle Doctor Story
By Paul White © 1981
Original text adapted from Paul White, 1981 ©. Used with permission.
Licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.

Illustrator: Tim Holden, BTL, Kenya 2003 ©, licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.

Template and Music © 2022 Wycliffe Bible Translators, Inc., licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.";



				case CreditsType.PaulWhiteNessAndHolden:
					return @"Based on the Jungle Doctor Story
By Paul White © 1981
Original text adapted from Paul White, 1981 ©. Used with permission.
Licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.

Illustrators: April Ness and Tim Holden, BTL, Kenya 2003 ©, licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.

Template and Music © 2022 Wycliffe Bible Translators, Inc., licensed under the terms of a Creative Commons Attribution-ShareAlike 4.0 International license.";




				case CreditsType.Unknown:
					return "This is just a demo. We need to add real credits.";

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void AddToOrUpdateCreditSlideMap(string md5Hash, string imagePath)
		{
			if (CreditSlideMap.TryGetValue(md5Hash, out var slideList))
			{
				if (slideList == null)
					slideList = new List<string>();
				slideList.Add(imagePath);
			}
			else
			{
				CreditSlideMap.Add(md5Hash, new List<string> {imagePath});
			}
		}

		public static void CreateMapFile()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var pair in CreditSlideMap)
			{
				foreach (var list in pair.Value)
					sb.Append(pair.Key).Append(" ").Append(list).Append(Environment.NewLine);
			}
			File.WriteAllText("mapFile.txt", sb.ToString());
		}
	}
}
