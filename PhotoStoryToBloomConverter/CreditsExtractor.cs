using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStoryToBloomConverter
{
    class CreditsExtractor
    {
        private static readonly string[] creditImageHashes = { "369F414F6F2CC91AE0AD0FF1CDCFAD21", "CDF13EC119AD0128E1196DB518B64BF8", "E3BF24BB5F62DFF39E7C2301E4ED2642", "56C6BDF749C9FE421C9BC6F832B7CCE0" };
        private static readonly string[] coverImageHashes = { "8C7B5AADFF9AB8B4649481421EB8479F", "781ED3E63E6BD138D9BE59A24EFF7D6A" };

        public static string extractedCreditString;

        //We are assuming that if an image is checked, it is part of the current book, and the credits should be extracted
        public static bool imageIsCreditsOrCover(string imagePath)
        {
            var md5Hash = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(imagePath))).Replace("-", "");
            //Currently all of the information from the credit page needs to be stored in the additional acknowledgments section
            if (string.Equals(md5Hash, creditImageHashes[0]))
            {
                /* Stories adapted by VM Productions from
                 * READ-N-GROW PICTURE BIBLE
                 * and used by permission.
                 * Illustrations by Jim Padgett, Courtesy of
                 * Sweet Publishing, Ft. Worth, TX. ©2011
                 * and Carolyn Dyk.
                 * Skin-darkened by Lori MacLean and VM Productions. */
                extractedCreditString = "Stories adapted by VM Productions from READ-N-GROW PICTURE BIBLE and used by permission." + " " +
                                        "Illustrations by Jim Padgett, Courtesy of Sweet Publishing, Ft. Worth, TX. ©2011 and Carolyn Dyk." + " " +
                                        "Skin-darkened by Lori MacLean and VM Productions.";
                return true;
            }
            else if (string.Equals(md5Hash, creditImageHashes[1]))
            {
                /* Stories adapted by VM Productions from
                 * READ-N-GROW PICTURE BIBLE
                 * and used by permission.
                 * Illustrations by Jim Padgett, Courtesy of
                 * Sweet Publishing, Ft. Worth, TX. ©2011
                 * and Carolyn Dyk.
                 * Skin-darkened by Lori MacLean and VM Productions.
                 * Some images adapted by Beth Rupprecht. */
                extractedCreditString = "Stories adapted by VM Productions from READ-N-GROW PICTURE BIBLE and used by permission." + " " +
                                        "Illustrations by Jim Padgett, Courtesy of Sweet Publishing, Ft. Worth, TX. ©2011 and Carolyn Dyk." + " " +
                                        "Skin-darkened by Lori MacLean and VM Productions." + " " +
                                        "Some images adapted by Beth Rupprecht.";
                return true;
            }
            //SinEnters and Ruth have the same credits, they are just different resolution images
            else if (string.Equals(md5Hash, creditImageHashes[2]) || string.Equals(md5Hash, creditImageHashes[3]))
            {
                /* Biblical Illustrations by Jim Padgett
                 * courtesy of Sweet Publishing, Ft. Worth, TX
                 * and Gospel Light, Ventura, CA
                 * Copyright 1984. All rights reserved.
                 * Additional illustrations by Carolyn Dyk. */
                extractedCreditString = "Biblical Illustrations by Jim Padgett courtesy of Sweet Publishing, Ft. Worth, TX and Gospel Light, Ventura, CA Copyright 1984. All rights reserved." + " " +
                                        "Additional illustrations by Carolyn Dyk.";
                return true;
            }

            foreach (var coverImageHash in coverImageHashes)
            {
                if (string.Equals(md5Hash, coverImageHash))
                    return true;
            }
            return false;
        }
    }
}
