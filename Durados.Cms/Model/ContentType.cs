using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class ContentType
    {
        public List<Css> GetCsss()
        {
            Dictionary<int, Css> csss = new Dictionary<int, Css>();

            if (!this.ContentTypeCss.IsLoaded)
                this.ContentTypeCss.Load();

            foreach (ContentTypeCss contentTypeCss in this.ContentTypeCss)
            {
                if (!contentTypeCss.CssReference.IsLoaded)
                    contentTypeCss.CssReference.Load();

                csss.Add(contentTypeCss.Css.ID, contentTypeCss.Css);
            }
            
            
            return csss.Values.ToList();

        }


        public List<Script> GetScripts()
        {
            Dictionary<int, Script> scripts = new Dictionary<int, Script>();

            if (!this.ContentTypeScript.IsLoaded)
                this.ContentTypeScript.Load();

            foreach (ContentTypeScript contentTypeScript in this.ContentTypeScript)
            {
                if (!contentTypeScript.ScriptReference.IsLoaded)
                    contentTypeScript.ScriptReference.Load();

                scripts.Add(contentTypeScript.Script.ID, contentTypeScript.Script);
            }
            


            return scripts.Values.ToList();
        }
    }
}
