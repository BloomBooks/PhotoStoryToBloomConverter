using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using SIL.Windows.Forms.ClearShare;
using SIL.Windows.Forms.ImageToolbox;

namespace PhotoStoryToBloomConverter.Utilities
{
	internal enum ImageCreator
	{
		Unknown,
		Dyk,
		Padgett
	}

	internal static class ImageUtilities
	{
		public static void ApplyImageIpInfo(string bookName, string imageLocation, CreditsAndCoverExtractor.CreditsType creditsType)
		{
			using (var image = PalasoImage.FromFile(imageLocation))
			{
				switch (creditsType)
				{
					case CreditsAndCoverExtractor.CreditsType.SweetPublishing:
					case CreditsAndCoverExtractor.CreditsType.SweetPublishingWithOttScript:
						ApplySweetPublishingIpInfoForImages(image);
						break;
					case CreditsAndCoverExtractor.CreditsType.SweetPublishingAndWycliffe:
					case CreditsAndCoverExtractor.CreditsType.SweetPublishingAndWycliffeWithOttScript:
						switch (GetImageCreator(image.OriginalFilePath))
						{
							case ImageCreator.Dyk:
								ApplyWycliffeIpInfoForImages(image);
								break;
							case ImageCreator.Padgett:
								ApplySweetPublishingIpInfoForImages(image);
								break;
							case ImageCreator.Unknown:
								Console.WriteLine($@"ERROR: Unable to determine image credits for {bookName}, {image.FileName}");
								break;
						}
						break;
					case CreditsAndCoverExtractor.CreditsType.PaulWhiteAndHolden:
						Apply(image, "© 2003 BTL, Kenya", "https://creativecommons.org/licenses/by-sa/4.0/", "Tim Holden");
						break;
					case CreditsAndCoverExtractor.CreditsType.PaulWhiteNessAndHolden:
						Apply(image, "© 2003 BTL, Kenya", "https://creativecommons.org/licenses/by-sa/4.0/", "April Ness and Tim Holden");
						break;
					case CreditsAndCoverExtractor.CreditsType.LostCoinRheburg:
						Apply(image, "© Wycliffe Bible Translators, Inc.", "https://creativecommons.org/licenses/by-sa/4.0/", "Judith Rheburg");
						break;
					case CreditsAndCoverExtractor.CreditsType.Kande:
						Apply(image, "© 2006 SIL International", "https://creativecommons.org/licenses/by-sa/4.0/", "MBANJI Bawe Ernest");
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(creditsType), creditsType, null);
				}

				image.SaveUpdatedMetadataIfItMakesSense();
			}
		}

		private static ImageCreator GetImageCreator(string imagePath)
		{
			var imageMd5Hash = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(imagePath))).Replace("-", "");
			if (ImageHashToCreatorDictionary.ContainsKey(imageMd5Hash))
				return ImageHashToCreatorDictionary[imageMd5Hash];
			return ImageCreator.Unknown;
		}

		private static void ApplySweetPublishingIpInfoForImages(PalasoImage image)
		{
			image.Metadata.CopyrightNotice = "© Sweet Publishing";
			image.Metadata.License = CreativeCommonsLicense.FromLicenseUrl("https://creativecommons.org/licenses/by-sa/4.0/");
			image.Metadata.Creator = "Jim Padgett (may have been skin-darkened or otherwise adapted by Wycliffe Bible Translators, Inc.)";
		}

		private static void ApplyWycliffeIpInfoForImages(PalasoImage image)
		{
			image.Metadata.CopyrightNotice = "© Wycliffe Bible Translators, Inc.";
			image.Metadata.License = CreativeCommonsLicense.FromLicenseUrl("https://creativecommons.org/licenses/by-nc-nd/4.0/");
			image.Metadata.License.RightsStatement = "You may crop and resize but not modify the images for your new work. " +
			                                         "Images may be rotated or flipped horizontally, provided this does not contradict historical fact or violate cultural norms.";
			image.Metadata.Creator = "Carolyn Dyk";
		}

		private static void Apply(PalasoImage image, string copyrightNotice, string licenseUrl, string creator, string rightsStatement = null)
		{
			image.Metadata.CopyrightNotice = copyrightNotice;
			image.Metadata.License = CreativeCommonsLicense.FromLicenseUrl(licenseUrl);
			image.Metadata.Creator = creator;
			image.Metadata.License.RightsStatement = rightsStatement;
		}

		// Unfortunate we can't get this information from the project in some consistent way.
		// For some of the images, the "comment" field includes the original file name but not all.
		// (If we knew all the original file names, we would know which ones are Wycliffe because they end in 'CD'.)
		// So we go with the poor man's approach which is tedious but works for now.
		private static readonly Dictionary<string, ImageCreator> ImageHashToCreatorDictionary = new Dictionary<string, ImageCreator>
		{
			{"8568F7AD2ADA9E55938342C5F809630F", ImageCreator.Dyk }, // 001 God Creates The World / 2.jpg
			{"4A0EBCCB677C0A5883533FADE1CD4614", ImageCreator.Dyk }, // 001 God Creates The World / 3.jpg
			{"91972225FEAC1007E54460F4FE5FC4D0", ImageCreator.Dyk }, // 001 God Creates The World / 5.jpg
			{"55409ACDA2DF12F8D76363FC2F64EE54", ImageCreator.Dyk }, // 001 God Creates The World / 7.jpg
			{"1A182623B49E48F181C10F1430A8C91B", ImageCreator.Dyk }, // 001 God Creates The World / 9.jpg
			{"611073D6C47281FDFD016DA624441C0A", ImageCreator.Dyk }, // 001 God Creates The World / 11.jpg
			{"EB923008DF17F6311957A340D25D6F00", ImageCreator.Dyk }, // 001 God Creates The World / 13.jpg
			{"F445843247C354E8A67EAAC65BB05890", ImageCreator.Dyk }, // 001 God Creates The World / 14.jpg
			{"A36CC915FDADBA155CF48558BDDA4658", ImageCreator.Dyk }, // 001 God Creates The World / 15.jpg
			{"6EFCFE003585F3CD5496098BD5289CB5", ImageCreator.Dyk }, // 001 God Creates The World / 16.jpg
			{"C6E844B4A09489E2C85D9B2B3467A802", ImageCreator.Dyk }, // 001 God Creates The World / 18.jpg
			{"02DA67558C0BA2463094966FEF7EAE49", ImageCreator.Dyk }, // 001 God Creates The World / 19.jpg
			{"789113789EA0F146F5EF832514EC3439", ImageCreator.Dyk }, // 001 God Creates The World / 20.jpg
			{"74DF9AA5C14CDCE2B5EACEEA2110D1EB", ImageCreator.Dyk }, // 001 God Creates The World / 21.jpg
			{"53CBE746274CD8AE40C331DC198F997F", ImageCreator.Dyk }, // 001 God Creates The World / 23.jpg
			{"93674FF9BA7E3B8F97FC6615777F268A", ImageCreator.Dyk }, // 001 God Creates The World / 25.jpg
			{"3371A91B46BC19D29644AFDCC3C4E502", ImageCreator.Dyk }, // 001 God Creates The World / 38.jpg

			{"65D90504562BD3BB568BAA42386434C1", ImageCreator.Padgett }, // 001 God Creates The World / 1.jpg
			{"D32E326CAB49A65678AB5377526B5CB1", ImageCreator.Padgett }, // 001 God Creates The World / 4.jpg
			{"D106F89141FEB9F0D1DC36AD09C69103", ImageCreator.Padgett }, // 001 God Creates The World / 6.jpg
			{"4452C4175AFDCB8C8F0B08FA3D8BE298", ImageCreator.Padgett }, // 001 God Creates The World / 8.jpg
			{"4D24DAA5CEB6D79DAE951CE22F14C936", ImageCreator.Padgett }, // 001 God Creates The World / 10.jpg
			{"C9AD73110DD6ED824FCB1E7669201E26", ImageCreator.Padgett }, // 001 God Creates The World / 12.jpg
			{"0D6B4B51029F90BB7C37318FB7CD2284", ImageCreator.Padgett }, // 001 God Creates The World / 17.jpg
			{"631E58AF5C90D20B1452009F777D9575", ImageCreator.Padgett }, // 001 God Creates The World / 22.jpg
			{"190E97DFBB2854DEE453DD2DEB568C1E", ImageCreator.Padgett }, // 001 God Creates The World / 24.jpg
			{"A3AC04827868706AF655F4F6B5A7D7AF", ImageCreator.Padgett }, // 001 God Creates The World / 26.jpg
			{"9EEEC8B4998A8AD344C8388531C9309A", ImageCreator.Padgett }, // 001 God Creates The World / 27.jpg
			{"963A8413B6FE780C1E731C1167E0A3F4", ImageCreator.Padgett }, // 001 God Creates The World / 28.jpg
			{"50858369298621DDC3C320DBF1F43900", ImageCreator.Padgett }, // 001 God Creates The World / 29.jpg
			{"CB22D6D87CB9459EBE6627D6D6A01C5E", ImageCreator.Padgett }, // 001 God Creates The World / 30.jpg
			{"DD4375D82A8B73A8B1221FD7EB0DCD78", ImageCreator.Padgett }, // 001 God Creates The World / 31.jpg
			{"8F0DCBC978B5ECC0D9C691E2108347A8", ImageCreator.Padgett }, // 001 God Creates The World / 32.jpg
			{"1FE881512804CDDEEA185A9D9F2F27DC", ImageCreator.Padgett }, // 001 God Creates The World / 33.jpg
			{"BDE25EBEA67AA65D2182508E6735D2FC", ImageCreator.Padgett }, // 001 God Creates The World / 34.jpg
			{"2631CC8C51E59809C313A91D05923602", ImageCreator.Padgett }, // 001 God Creates The World / 35.jpg
			{"401C4E7934F0CF415748394C4A5FE2C5", ImageCreator.Padgett }, // 001 God Creates The World / 36.jpg
			{"2C98A2FC585D3616FD01E5EEA4717338", ImageCreator.Padgett }, // 001 God Creates The World / 37.jpg

			{"FF1BD89F54BD73541292916B9AD65978", ImageCreator.Dyk }, // 002 The Fall into Sin / 5.jpg

			{"74828A5DCA97F6BEACAF94A8DB75EB33", ImageCreator.Padgett }, // 002 The Fall into Sin / 2.jpg
			{"216C065C07B2DBE77C33AB96A6BF5974", ImageCreator.Padgett }, // 002 The Fall into Sin / 4.jpg
			{"DE56A1FB29ACCE0A0F4898E2420E70AB", ImageCreator.Padgett }, // 002 The Fall into Sin / 6.jpg, 9.jpg
			{"B9DA5CDAB736E21AF2BE9E09EA0AA1A3", ImageCreator.Padgett }, // 002 The Fall into Sin / 7.jpg, 10.jpg
			{"D1BA47112F275178FFAA87054F9871D7", ImageCreator.Padgett }, // 002 The Fall into Sin / 8.jpg
			{"FB5BE857D17DA4DF9963843E4D916F40", ImageCreator.Padgett }, // 002 The Fall into Sin / 11.jpg
			{"79F8FFF4BEE3A654DFFB29A6F4392965", ImageCreator.Padgett }, // 002 The Fall into Sin / 12.jpg
			{"5B9FAD67618604E98F4E607ECABD1845", ImageCreator.Padgett }, // 002 The Fall into Sin / 13.jpg, 21.jpg
			{"30F0E714872296D5EBA15CD1CCDC1166", ImageCreator.Padgett }, // 002 The Fall into Sin / 14.jpg, 16.jpg, 17.jpg
			{"AEBA571D5ADD3F28090C5A6F36BF2110", ImageCreator.Padgett }, // 002 The Fall into Sin / 15.jpg, 18.jpg
			{"3ED11A4680AD8EB1FA45A98FCF0BF362", ImageCreator.Padgett }, // 002 The Fall into Sin / 19.jpg
			{"576F2E507A2EB1F79EC204418E8D000F", ImageCreator.Padgett }, // 002 The Fall into Sin / 20.jpg
			{"F94E6B63447AB283F4E523601FDEA1D7", ImageCreator.Padgett }, // 002 The Fall into Sin / 22.jpg
			{"0EABD7B92F4B1D97D1DEB6DC6F5875A7", ImageCreator.Padgett }, // 002 The Fall into Sin / 23.jpg, 24.jpg
			{"13489D7D084D2A0FAD69A9904F137286", ImageCreator.Padgett }, // 002 The Fall into Sin / 25.jpg
			{"BCE6C7195768EB75C53730E7D4D5C4D1", ImageCreator.Padgett }, // 002 The Fall into Sin / 26.jpg
			{"098775A22F244BC2E8205C1DD1F656D5", ImageCreator.Padgett }, // 002 The Fall into Sin / 27.jpg
			{"6C22952E50B30FE6201DBE4829BFD533", ImageCreator.Padgett }, // 002 The Fall into Sin / 28.jpg
			{"692569FFC8E15DA8D30D19D68F2F4B40", ImageCreator.Padgett }, // 002 The Fall into Sin / 29.jpg
			{"D8CC91DC42998AEC918BE0C3D47DD7F8", ImageCreator.Padgett }, // 002 The Fall into Sin / 30.jpg
			{"EF7D22853ED3D4612146D7C174CCE3BA", ImageCreator.Padgett }, // 002 The Fall into Sin / 31.jpg
			{"A3FD07EB1FE76DD129998BD078C09A2E", ImageCreator.Padgett }, // 002 The Fall into Sin / 32.jpg, 37.jpg, 39.jpg
			{"3B3DF970A9CFA59ACB7D29EE6124AFAA", ImageCreator.Padgett }, // 002 The Fall into Sin / 33.jpg
			{"1EACB871F1A88400064E9E06606C096A", ImageCreator.Padgett }, // 002 The Fall into Sin / 34.jpg
			{"8E7E3AF14A4599E53FDD24FE791A5DF4", ImageCreator.Padgett }, // 002 The Fall into Sin / 35.jpg
			{"FDE3395E8EBDF9CE81DB46AE58C76CBC", ImageCreator.Padgett }, // 002 The Fall into Sin / 36.jpg
			{"FB873B4486E086A6C733E72F01F860F4", ImageCreator.Padgett }, // 002 The Fall into Sin / 38.jpg
			{"75B3ABE134A62458F3B7EA13EE84F36E", ImageCreator.Padgett }, // 002 The Fall into Sin / 40.jpg

			{"8EF4CAD6DEEEAEE0DC4F1E682FA4D927", ImageCreator.Dyk }, // 100 Beginning Creation / 2.jpg
			{"42CF835F0AEE04B329C15C49D8B9A117", ImageCreator.Dyk }, // 100 Beginning Creation / 19.jpg

			{"C165F60FD68A9C4C042CEE6B14E7FB2B", ImageCreator.Padgett }, // 100 Beginning Creation / 1.jpg
			{"3696F955321DA398E3646C208FB27AE9", ImageCreator.Padgett }, // 100 Beginning Creation / 15.jpg
			{"41499318CA7EB5330BCB4716627B5280", ImageCreator.Padgett }, // 100 Beginning Creation / 17.jpg
			{"FF3377D215C5C8450E12589494171152", ImageCreator.Padgett }, // 100 Beginning Creation / 18.jpg
			{"FF2073F30836BA0AE03023EB93C29879", ImageCreator.Padgett }, // 100 Beginning Creation / 20.jpg
			{"6BE6AF9EBB5D3EB86632DAE57672BCD2", ImageCreator.Padgett }, // 100 Beginning Creation / 21.jpg, 22.jpg, 23.jpg
			{"0CAA445C083EA72CAA5CB486F1AF4973", ImageCreator.Padgett }, // 100 Beginning Creation / 26.jpg
			{"BE11FA8CF365DC21CFA0ED00E620D02A", ImageCreator.Padgett }, // 100 Beginning Creation / 27.jpg
		};
	}
}
