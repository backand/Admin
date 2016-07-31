using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class PageContent
    {
        public string PlaceHolderName
        {
            get
            {
                if (!this.PlaceHolderReference.IsLoaded)
                    this.PlaceHolderReference.Load();
                return this.PlaceHolder.Name;
            }
        }

        
    }
}
