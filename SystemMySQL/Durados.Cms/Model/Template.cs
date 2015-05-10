using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class Template
    {
        public List<Css> GetCsss()
        {
            Dictionary<int, Css> csss = new Dictionary<int, Css>();

            if (!this.TemplateCss.IsLoaded)
                this.TemplateCss.Load();

            foreach (TemplateCss contentTypeCss in this.TemplateCss)
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

            if (!this.TemplateScript.IsLoaded)
                this.TemplateScript.Load();

            foreach (TemplateScript contentTypeScript in this.TemplateScript)
            {
                if (!contentTypeScript.ScriptReference.IsLoaded)
                    contentTypeScript.ScriptReference.Load();

                scripts.Add(contentTypeScript.Script.ID, contentTypeScript.Script);
            }
            


            return scripts.Values.ToList();
        }
    }
}
