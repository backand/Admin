using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Ajax;

namespace System.Web.Mvc
{
    public static class ImageActionLinkHelper
    {
        public static string ImageActionLink(this AjaxHelper helper, string imageUrl, string altText, string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("title", altText);
            var link = helper.ActionLink("[replaceme]", actionName, routeValues, ajaxOptions);
            return link.ToString().Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));
        }

        public static string ImageActionLink(this AjaxHelper helper, string className, string altText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions)
        {
            var builder = new TagBuilder("span");
           // builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("title", altText);
            builder.MergeAttribute("class", "img " + className);
            var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions);
            return link.ToString().Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        /// by miri
        /// Return <a> to disabled links 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="imageUrl"></param>
        /// <param name="altText"></param>
        /// <param name="actionName"></param>
        /// <param name="routeValues"></param>
        /// <param name="ajaxOptions"></param>
        /// <returns></returns>
        public static string ImageLink(this AjaxHelper helper, string className)
        {
            var link="<a>[replaceme]</a>";
            var builder = new TagBuilder("span");
            builder.MergeAttribute("class", "img disabled " + className);
            return link.Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}
