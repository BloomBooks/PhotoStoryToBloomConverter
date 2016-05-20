using System.Collections.Generic;
using System.Linq;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to the 'Html' object
    public class BloomDocument
    {
        private BloomMetadata _metadata;
        private BloomBookData _bookData;
        private readonly List<BloomPage> _pages = new List<BloomPage>();

        public void AddPage(BloomPage page)
        {
            _pages.Add(page);
        }

        public static BloomDocument DefaultPhotoStoryConvertedBloomDocument(string bookName)
        {
            return new BloomDocument {_metadata = BloomMetadata.DefaultBloomMetadata(bookName), _bookData = BloomBookData.DefaultBloomBookData(bookName)};
        }

        public Html ConvertToHtml()
        {
            var html = new Html
            {
                Head = _metadata.ConvertToHtml(),
                Body = new Body
                {
                    BookCreationType = "original",
                    Class = "publishMode",
                    Divs = new List<Div>
                    {
                        _bookData.ConvertToHtml(),
                    }
                }
            };
            html.Body.Divs.AddRange(_pages.Select(page => page.ConvertToHtml()));
            return html;

        }
    }
}