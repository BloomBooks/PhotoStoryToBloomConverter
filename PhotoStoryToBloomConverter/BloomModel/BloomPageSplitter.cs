using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using System.Collections.Generic;
using System.IO;

namespace PhotoStoryToBloomConverter.BloomModel
{
    public class BloomPageSplitter
    {
        public BloomTextBox TextBox;
        public BloomImage Image;
        public BloomAudio Audio;

        public Div ConvertToHtml()
        {
            var imageDiv = Image.ConvertToHtml();
            //var textDiv = TextBox.ConvertToHtml();
            var narrationSpan = (Audio.NarrationPath == null)? null : new Span { Class = "audio-sentence", Id = Path.GetFileNameWithoutExtension(Audio.NarrationPath), RecordingMD5 = "undefined", ContentText = "" };
            return new Div
            {
                Class = "split-pane horizontal-percent",
                Style = "min-height: 42px;",
                Divs = new List<Div>
                {
                    new Div
                    {
                        Class = "split-pane-component position-top",
                        Style = "bottom: 0%;",
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
                                    imageDiv,
                                }
                            }
                        }
                    },
                    new Div
                    {
                        Class = "split-pane-divider horizontal-divider",
                        Style = "bottom: 0%;",
                        Title = "100%",
                        SimpleText = ""
                    },
                    new Div
                    {
                        Class = "split-pane-component position-bottom",
                        Style = "height: 0%;",
                        Divs = new List<Div>
                        {
                            new Div 
                            {
                                Class = "split-pane-component-inner adding",
                                Style = "position: relative;",
                                MinWidth = "60px 150px 250px",
                                Divs = new List<Div>
                                {
                                    new Div
                                    {
                                        Class = "box-header-off bloom-translationGroup",
                                        Divs = new List<Div>
                                        {
                                            new Div
                                            {
                                                Class = "bloom-editable cke_editablecke_editable_inline cke_contents_ltr bloom-content1",
                                                ContentEditable = "true",
                                                Lang = "en",
                                                TabIndex = "0",
                                                Spellcheck = "true",
                                                Role = "textbox",
                                                AriaLabel = "false",
                                                FormattedText = new Paragraph
                                                {
                                                    Text = ""
                                                }
                                            }
                                        }
                                    },
                                    new Div
                                    {
                                        Class = "bloom-translationGroup bloom-trailingElement normal-style childOverflowingThis",
                                        Divs = new List<Div>
                                        {
                                            new Div
                                            {
                                                Class = "bloom-editable cke_editable cke_editable_inline cke_contents_ltr normal-style thisOverflowingParent bloom-content1",
                                                ContentEditable = "true",
                                                Lang = "en",
                                                Style = "min-height: 24px;",
                                                TabIndex = "0",
                                                Spellcheck = "true",
                                                Role = "textbox",
                                                AriaLabel = "false",
                                                DataLanguageTipContent = "English",
                                                FormattedText = new Paragraph
                                                {
                                                    Span = narrationSpan
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
