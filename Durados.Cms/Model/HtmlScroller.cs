using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class HtmlScroller
    {
        public List<HtmlScrollerContent> GetContents()
        {
            if (!this.HtmlScrollerContent.IsLoaded)
                this.HtmlScrollerContent.Load();
            foreach (HtmlScrollerContent content in HtmlScrollerContent)
            {
                if (!content.ContentReference.IsLoaded)
                    content.ContentReference.Load();
            }
            return this.HtmlScrollerContent.ToList();
        }
    }
}
