using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class Page
    {
        public string TemplateName
        {
            get
            {
                if (Template == null)
                {
                    TemplateReference.Load();
                }

                return Template.Name;
            }
        }

        public List<Script> GetScripts()
        {
            Dictionary<int, Script> scripts = new Dictionary<int, Script>();

            if (!this.TemplateReference.IsLoaded)
                this.TemplateReference.Load();

            if (!this.Template.TemplateScript.IsLoaded)
                this.Template.TemplateScript.Load();

            foreach (TemplateScript templateScript in this.Template.TemplateScript)
            {
                if (!templateScript.ScriptReference.IsLoaded)
                    templateScript.ScriptReference.Load();

                if (!scripts.ContainsKey(templateScript.Script.ID))
                    scripts.Add(templateScript.Script.ID, templateScript.Script);
            }

            if (!this.PageContent.IsLoaded)
                this.PageContent.Load();

            foreach (PageContent pageContent in this.PageContent)
            {
                if (!pageContent.ContentReference.IsLoaded)
                    pageContent.ContentReference.Load();

                if (!pageContent.Content.ContentTypeReference.IsLoaded)
                    pageContent.Content.ContentTypeReference.Load();

                if (!pageContent.Content.ContentType.ContentTypeScript.IsLoaded)
                    pageContent.Content.ContentType.ContentTypeScript.Load();

                foreach (ContentTypeScript contentTypeScript in pageContent.Content.ContentType.ContentTypeScript.OrderBy(c=>c.Ordinal))
                {
                    if (!contentTypeScript.ScriptReference.IsLoaded)
                        contentTypeScript.ScriptReference.Load();

                    if (!scripts.ContainsKey(contentTypeScript.Script.ID))
                        scripts.Add(contentTypeScript.Script.ID, contentTypeScript.Script);
                }
            }


            return scripts.Values.ToList();
        }

        public List<Css> GetCsss()
        {
            Dictionary<int, Css> csss = new Dictionary<int, Css>();

            if (!this.TemplateReference.IsLoaded)
                this.TemplateReference.Load();

            if (!this.Template.TemplateCss.IsLoaded)
                this.Template.TemplateCss.Load();

            foreach (TemplateCss templateCss in this.Template.TemplateCss)
            {
                if (!templateCss.CssReference.IsLoaded)
                    templateCss.CssReference.Load();

                if (!csss.ContainsKey(templateCss.Css.ID))
                    csss.Add(templateCss.Css.ID, templateCss.Css);
            }


            if (!this.PageContent.IsLoaded)
                this.PageContent.Load();

            foreach (PageContent pageContent in this.PageContent)
            {
                if (!pageContent.ContentReference.IsLoaded)
                    pageContent.ContentReference.Load();

                if (!pageContent.Content.ContentTypeReference.IsLoaded)
                    pageContent.Content.ContentTypeReference.Load();

                if (!pageContent.Content.ContentType.ContentTypeCss.IsLoaded)
                    pageContent.Content.ContentType.ContentTypeCss.Load();

                foreach (ContentTypeCss contentTypeCss in pageContent.Content.ContentType.ContentTypeCss)
                {
                    if (!contentTypeCss.CssReference.IsLoaded)
                        contentTypeCss.CssReference.Load();

                    if (!csss.ContainsKey(contentTypeCss.Css.ID))
                        csss.Add(contentTypeCss.Css.ID, contentTypeCss.Css);
                }
            }



            return csss.Values.ToList();
        }
    }
}
