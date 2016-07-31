using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class Fader
    {
        public Menu GetMenu()
        {
            if (!MenuReference.IsLoaded)
                MenuReference.Load();

            return Menu;
        }
        public List<FaderContent> GetContents()
        {
            if (!this.FaderContent.IsLoaded)
                this.FaderContent.Load();
            return this.FaderContent.ToList();
        }
    }
}
