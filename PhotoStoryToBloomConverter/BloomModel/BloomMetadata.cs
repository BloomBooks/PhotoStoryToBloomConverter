using System.Linq;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to the html 'Head' object
    public class BloomMetadata
    {
        public static string[] StandardBloomLinks =
        {
            "basePage.css",
            "languageDisplay.css",
            "previewMode.css",
            "origami.css",
            "Basic Book.css",
            "Traditional-XMatter.css",
            "..\\settingsCollectionStyles.css",
            "..\\customCollectionStyles.css"
        };

        public static string[] StandardBloomStyles =
        {
            ".BigWords-style { font-size: 45pt !important; text-align: center !important; }",
            "DIV.coverColor TEXTAREA { background-color: #C2A6BF !important; }\r\nDIV.bloom-page.coverColor { background-color: #C2A6BF !important }",
        };

        public static BloomMetadata DefaultBloomMetadata(string title)
        {
            return new BloomMetadata
            {
                Links = StandardBloomLinks,
                BloomVersion = "2.0",
                Charset = "UTF-8",
                TemplateSource = "Basic Book",
                Generator = "Bloom Version 3.6.123",
                Styles = StandardBloomStyles,
                Title = title,
                LockedDownAsShell = "true"
            };
        }

        public string[] Links;
        public string Script;
        public string Title;
        public string BloomVersion;
        public string Charset;
        public string TemplateSource;
        public string Generator;
        public string[] Styles;
        public string LockedDownAsShell;

        public Head ConvertToHtml()
        {
            return new Head
            {
                Title = new Title {TitleText = Title},
                Script = (string.IsNullOrEmpty(Script))? null : new Script {Src = Script, Type = "text/javascript"},
                Links = Links.Select(linkRef => new Link {Href = linkRef, Rel = "stylesheet", Type = "text/css"}).ToArray(),
                Styles = Styles.Select(styleCss => new Style {Css = styleCss, Type = "text/css"}).ToArray(),
                Metas = new []
                {
                    new Meta { Charset = Charset },
                    new Meta { Name = "Generator", Content = Generator },
                    new Meta { Name = "BloomFormatVersion", Content = BloomVersion },
                    new Meta { Name = "pageTemplateSource", Content = TemplateSource },
                    new Meta { Name = "lockedDownAsShell", Content = LockedDownAsShell }
                }
            };
        }
    }
}