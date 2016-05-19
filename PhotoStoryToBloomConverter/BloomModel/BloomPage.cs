using System;
using System.Collections.Generic;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to a div with the bloom tag 'bloom-page'
    public class BloomPage
    {
        public BloomTextBox TextBox;
        public BloomImage Image;

        public string BloomTags;
        public string Uuid;
        public string DataPageLineage;
        public string Language;

        public string PageLabel;
        public string PageDescription;

        public BloomPage()
        {
            Uuid = Guid.NewGuid().ToString();
        }

        public static BloomPage DefaultBloomPageWithImage(BloomImage image)
        {
            return new BloomPage
            {
                BloomTags = "bloom-page numberedPage customPage A4Landscape layout-style-Default bloom-monolingual",
                Image = image
            };
        }

        public Div ConvertToHtml()
        {
            return new Div
            {
                Class = BloomTags,
                Id = Uuid,
                DataPageLineage = "056B6F11-4A6C-4942-B2BC-8861E62B03B3",
                Lang = "",
                Divs = new List<Div>
                {
                    new Div {Class = "pageLabel", DataI18n = "", Lang = Language, SimpleText = PageLabel,},
                    new Div {Class = "pageDescription", Lang = Language, SimpleText = PageDescription},
                    new Div
                    {
                        Class = "marginBox",
                        Divs = new List<Div>
                        {
                            new Div
                            {
                                Class = "split-pane-component-inner",
                                Style = "position: relative;",
                                MinHeight = "60px 150px 250px",
                                MinWidth = "60px 150px 250px",
                                Divs = new List<Div>
                                {
                                    Image.ConvertToHtml(),
                                    new Div
                                    {
                                        Class = "resize-sensor",
                                        Style =
                                            "position: absolute; left: 0px; top: 0px; right: 0px; bottom: 0px; overflow: hidden; z-index: -1; visibility: hidden;",
                                        Divs = new List<Div>
                                        {
                                            new Div
                                            {
                                                Class = "resize-sensor-expand",
                                                Style =
                                                    "position: absolute; left: 0; top: 0; right: 0; bottom: 0; overflow: hidden; z-index: -1; visibility: hidden;",
                                                Divs = new List<Div>
                                                {
                                                    new Div
                                                    {
                                                        Style =
                                                            "position: absolute; left: 0px; top: 0px; transition: all 0s ease 0s; width: 418px; height: 347px;"
                                                    }
                                                }
                                            },
                                            new Div
                                            {
                                                Class = "resize-sensor-shrink",
                                                Style =
                                                    "position: absolute; left: 0; top: 0; right: 0; bottom: 0; overflow: hidden; z-index: -1; visibility: hidden;",
                                                Divs = new List<Div>
                                                {
                                                    new Div
                                                    {
                                                        Style =
                                                            "position: absolute; left: 0; top: 0; transition: 0s; width: 200%; height: 200%"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}