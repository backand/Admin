using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Durados.Web.Mvc;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Infrastructure;
using Durados.Web.Mvc.UI.Json;

namespace Durados.Web.Mvc.UI
{
    public class ColumnFieldViewer : FieldViewer
    {


        public override HtmlControlType GetHtmlControlType(Field field)
        {
            return GetHtmlControlType((ColumnField)field);
        }

        public virtual HtmlControlType GetHtmlControlType(ColumnField field)
        {
            //todo: Read the HtmlControlType from the XML
            if (field.Custom)
                return HtmlControlType.Hidden;
            else if (field.ColumnFieldType == ColumnFieldType.Boolean)
                if (field.BooleanHtmlControlType == BooleanHtmlControlType.Radio)
                    return HtmlControlType.Radio;
                else
                    return HtmlControlType.Check;
            else if (field.Upload != null && !(field.DataColumn.DataType.Equals(typeof(string)) && field.TextHtmlControlType == TextHtmlControlType.TextArea && field.Rich))
            {
                return HtmlControlType.Upload;
            }
            else if (field.FtpUpload != null && !(field.DataColumn.DataType.Equals(typeof(string)) && field.TextHtmlControlType == TextHtmlControlType.TextArea && field.Rich))
            {
                return HtmlControlType.Upload;
            }
            else if (field.DataType == DataType.Image && !(field.DataColumn.DataType.Equals(typeof(string)) && field.TextHtmlControlType == TextHtmlControlType.TextArea && field.Rich))
            {
                return HtmlControlType.Upload;
            }
            else if (field.EnumType != null || field.TextHtmlControlType == TextHtmlControlType.DropDown || field.TextHtmlControlType == TextHtmlControlType.DependencyCustom)
            {
                return HtmlControlType.DropDown;
            }
            else if (field.DataColumn.DataType.Equals(typeof(string)) && field.TextHtmlControlType == TextHtmlControlType.TextArea)
            {
                return HtmlControlType.TextArea;
            }
            else if (field.DataColumn.DataType.Equals(typeof(string)) && field.TextHtmlControlType == TextHtmlControlType.Autocomplete)
            {
                return HtmlControlType.AutocompleteColumn;
            }
            else if (field.DataColumn.DataType.Equals(typeof(string)) && field.TextHtmlControlType == TextHtmlControlType.Url)
            {
                return HtmlControlType.Url;
            }
            else if (field.DataColumn.DataType.Equals(typeof(string)) && field.TextHtmlControlType == TextHtmlControlType.ColorPicker)
            {
                return HtmlControlType.ColorPicker;
            }
            else if (field.DataColumn.DataType.Equals(typeof(string)) && field.TextHtmlControlType == TextHtmlControlType.CheckList)
            {
                return HtmlControlType.CheckList;
            }
            else if (field.DataColumn.DataType.Equals(typeof(string)) && field.TextHtmlControlType == TextHtmlControlType.Milestone)
            {
                return HtmlControlType.Milestone;
            }
            else
                return HtmlControlType.Text;
        }

        public override string GetElementForTableView(Field field, DataRow dataRow, string guid)
        {
            return GetElementForTableView((ColumnField)field, dataRow, guid);
        }

        public virtual string GetElementForTableView(ColumnField field, DataRow dataRow, string guid)
        {
            switch (GetHtmlControlType(field))
            {
                case HtmlControlType.Upload:
                    return GetUploadForTableView(field, dataRow[field.DataColumn.ColumnName], field.View.GetPkValue(dataRow), guid);
                case HtmlControlType.Check:
                    return GetCheckForTableView(field, dataRow[field.DataColumn.ColumnName], guid);
                case HtmlControlType.Radio:
                    return GetRadioForTableView(field, dataRow, guid);
                case HtmlControlType.TextArea:
                    return GetRichDialog(field, dataRow, guid);
                case HtmlControlType.Url:
                    return GetUrlForTableView(field, field.ConvertToString(dataRow), field.View.GetPkValue(dataRow), guid);
                case HtmlControlType.DropDown:
                    if (string.IsNullOrEmpty(field.EnumType))
                        return GetDropDownForTableView(field, dataRow, guid);
                    else
                        return field.ConvertToString(dataRow);
                //case HtmlControlType.Milestone:
                //      return GetMilestoneForTableView(field,dataRow,guid);
                default:
                    return field.ConvertToString(dataRow);
            }
        }

        public virtual string GetDropDownForTableView(ColumnField field, DataRow dataRow, string guid)
        {
            //View parentView = field.GetParentView();
            //if (parentView != null && ((View)field.GetParentView()).IsAllow())
            //    return "<a href='JavaScript:void(0);' onclick='ctrlNavigate(this, event);' d_role='parentTableView' d_url='" + GetNavigationUrl(field, dataRow) + "', colName='" + field.Name + "' pk='" + field.GetValue(dataRow) + "' >" + field.ConvertToString(dataRow) + "</a>";
            //else

            View view = field.GetParentView();
            //if (view != null && ((View)field.GetParentView()).IsAllow())

            string id = guid + inlineEditingPrefix + field.Name;
            string type = "tableView";

            //View view = (View)field.ParentView;
            string pkValue = field.GetValue(dataRow);
            string editClick = string.Empty;

            if (field.EditInTableView)
            {
                if (view.Popup && !Durados.Web.Infrastructure.General.IsMobile())
                {
                    return "<a href='JavaScript:void(0);' colName='" + field.Name + "' pk='" + pkValue + "' onclick=\"InlineEditingDialog.CreateAndOpen('" + view.Name + "','" + view.DisplayName + "','" + type + "',this ,'" + view.GetInlineEditingEditUrl() + "', '" + guid + "', event)\">" + field.ConvertToString(dataRow) + "</a>";
                }
                else
                {
                    string url = GetUrlWithoutQuery(view, "Item");
                    url += "?viewName=" + view.Name + "&pk=____&guid=" + view.Name + "_Item_";

                    editClick += "d_edit(null, " + view.Popup.ToString().ToLower() + ", " + Durados.Web.Infrastructure.General.IsMobile().ToString().ToLower() + ", '" + url + "', '" + view.Name + "', '" + guid + "', this, event)";

                    return "<a href='JavaScript:void(0);' colName='" + field.Name + "' pk='" + pkValue + "' onclick=\"" + editClick + "\">" + field.ConvertToString(dataRow) + "</a>";
                }
            }
            else
            {
                if (view == null && field.Name == "DisplayColumn")
                {
                    Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                    string viewName2 = configAccess.GetViewNameByPK(dataRow["ID"].ToString(), Map.GetConfigDatabase().ConnectionString);

                    Durados.Web.Mvc.View view2 = (Durados.Web.Mvc.View)Map.Database.Views[viewName2];
                    string displayFieldName = field.ConvertToString(dataRow);
                    if (string.IsNullOrEmpty(displayFieldName))
                        return string.Empty;
                    else
                        return view2.Fields[displayFieldName].DisplayName;
                }
                else if (view == null || !view.IsAllow() || field.NoHyperlink)
                    return field.ConvertToString(dataRow);
                else
                    return "<a href='JavaScript:void(0);' onclick='ctrlNavigate(this, event);' d_role='parentTableView' d_url='" + GetNavigationUrl(field, dataRow) + "', colName='" + field.Name + "' pk='" + field.GetValue(dataRow) + "' >" + field.ConvertToString(dataRow) + "</a>";

            }

            // return field.ConvertToString(dataRow);

        }

        protected virtual string GetNavigationUrl(ColumnField field, DataRow dataRow)
        {
            string url = GetUtlWithoutQuery(field);

            if (url == null)
                return null;

            View parentView = field.GetParentView();

            if (parentView == null)
                return null;

            if (parentView.DataTable.PrimaryKey.Length != 1)
                return null;

            string query = "?";
            if (string.IsNullOrEmpty(field.DropDownValueField))
            {
                query += parentView.DataTable.PrimaryKey[0].ColumnName + "=" + System.Web.HttpContext.Current.Server.UrlEncode(field.GetValue(dataRow)) + "&";
            }
            else if (field.View.Name == "View" && field.DropDownValueField == "Name")
            {
                string viewName = field.GetValue(dataRow);
                if (Map.Database.Views.ContainsKey(viewName))
                {
                    query += parentView.DataTable.PrimaryKey[0].ColumnName + "=" + System.Web.HttpContext.Current.Server.UrlEncode(Map.Database.Views[viewName].ID.ToString()) + "&";
                }
                else
                {
                    query += field.DropDownValueField + "=" + System.Web.HttpContext.Current.Server.UrlEncode(field.GetValue(dataRow)) + "&";
                }
            }
            else
            {
                query += field.DropDownValueField + "=" + System.Web.HttpContext.Current.Server.UrlEncode(field.GetValue(dataRow)) + "&";
            }

            query += "__" + field.Name + "__=" + System.Web.HttpContext.Current.Server.UrlEncode(field.ConvertToString(dataRow));

            return url + query;
        }

        protected virtual string GetUtlWithoutQuery(ColumnField field)
        {
            View parentView = field.GetParentView();

            if (parentView == null)
                return null;

            string url = System.Web.HttpContext.Current.Request.ApplicationPath;

            if (!url.EndsWith("/"))
                url += "/";
            url += parentView.Controller + "/";
            url += parentView.IndexAction + "/";
            url += parentView.Name;

            return url;
        }

        private string GetUrlForTableView(ColumnField field, string value, string pk, string guid)
        {
            //by br
            return GetUrl(field, inlineEditingPrefix, string.Empty, field.CssClass, true, value, pk, guid);
            //return GetUrl(field, inlineEditingPrefix, string.Empty, field.CssClass, field.IsDisableForEdit(guid) || field.IsHiddenInEdit(), value, pk, guid);
        }

        public virtual string GetRichDialog(ColumnField field, string pk, string part, string guid)
        {
            string rich = field.DisplayFormat == DisplayFormat.MultiLinesEditor ? "1" : "0";
            int textLength = field.PartialLength;
            string lengthAttribute = textLength > 0 ? "d_len='" + textLength + "'" : "";
            string html = "<div class='richTextContainer' preview='1' rich='" + rich + "' " + lengthAttribute + "><div class='richText'>";

            if (rich == "0" && !string.IsNullOrEmpty(part))
            {
                part = Durados.Web.Mvc.Infrastructure.General.StripTags(part);
                part = (part.Length > textLength && textLength > 0) ? part.Substring(0, textLength) + "..." : part;
            }

            html += part;
            html += "</div></div>";

            return html;
        }

        protected virtual string GetRichDialog(ColumnField field, DataRow dataRow, string guid)
        {
            string pk = field.View.GetPkValue(dataRow);
            string part = field.ConvertToString(dataRow);
            return GetRichDialog(field, pk, part, guid);
        }

        private string GetRadioForTableView(ColumnField field, DataRow dataRow, string guid)
        {

            string s = field.ConvertToString(dataRow);

            if (Map.Database.Localization != null)
                return Map.Database.Localizer.Translate(s);
            else
                return s;
        }

        private string GetUploadForTableView(ColumnField field, object value, string pk, string guid)
        {
            string uploadAction = string.Empty;
            string src = string.Empty;
            string format = field.DisplayFormat.ToString();
            string[] imageExtensions = new string[8] { "bmp", "png", "jpg", "jpeg", "gif", "tiff", "tif", "svg" };
            string title = string.Empty;
            string onload = " onload=\"Durados.Image.SetSize(this)\" ";

            if (string.IsNullOrEmpty(value.ToString()))
            {
                string fullUrl = field.FtpUpload == null && field.Upload == null ? " fullUrl='yes' " : "";
                /**imgLoadSelector (jQuery+css onload+display image selector solution) solution for image onload, onload is used here for displaying image when finishing loading, it doesn't work for Firefox*/
                return "<a><img style='display: none' preview='1' dialog='1' format='" + format + "' " + fullUrl + onload + " class='" + field.CssClass + " imgLoadSelector' border=0 colName='" + field.Name + "' id='" + guid + "table_" + field.Name.ReplaceNonAlphaNumeric() + "' name='" + guid + "table_" + field.Name + "' alt='" + title + "' title='" + title + "' /></a>";
            }

            if (field.Upload != null)
            {

                //uploadPath = field.Upload.GetFixedVirtualPath();
                if (field.Upload.UploadFileType == UploadFileType.Image)
                {
                    if (value != DBNull.Value)
                    {
                        src = field.Upload.UploadVirtualPath + field.Name;

                        string href = string.Empty;
                        //string src = string.Empty;
                        if (field.Upload.UploadStorageType == UploadStorageType.File)
                        {
                            href = field.Upload.GetFixedVirtualPath();
                            href = href + value.ToString().TrimStart('/');

                            string filename = value.ToString();
                            string[] segments = filename.Split('.');
                            string extension = segments.Length > 0 ? segments[segments.Length - 1] : string.Empty;

                            if (imageExtensions.Contains(extension.ToLower()))
                            {
                                if (Maps.MultiTenancy)
                                {
                                    string url = System.Web.HttpContext.Current.Request.ApplicationPath;

                                    //title = Map.Database.Localizer.Translate("Download") + " " + field.DisplayName;

                                    View view = (View)field.View;

                                    if (!url.EndsWith("/"))
                                        url += "/";
                                    url += view.Controller + "/";
                                    url += "Download/";
                                    url += view.Name;
                                    url += string.Format("?fieldName={0}&filename={1}", System.Web.HttpContext.Current.Server.UrlEncode(field.Name), System.Web.HttpContext.Current.Server.UrlEncode(value.ToString().Replace("\"", @"\" + "\"")));
                                    url += "&pk=" + pk;
                                    href = url;
                                }
                                src = href;

                            }
                            else
                            {
                                src = System.Web.HttpContext.Current.Request.ApplicationPath + "/Content/Images/file.gif";
                            }
                        }
                        else
                            src = GetDatabaseSrc();


                        title = field.Upload.Title;
                        if (title == null) title = string.Empty;
                        /**imgLoadSelector (jQuery+css onload+display image selector solution) solution for image onload, onload is used here for displaying image when finishing loading, it doesn't work for Firefox*/
                        return "<a href='" + href + "'><img preview='1' dialog='1' format='" + format + "' " + onload + " class='" + field.CssClass + " imgLoadSelector' border=0 colName='" + field.Name + "' id='" + guid + "table_" + field.Name.ReplaceNonAlphaNumeric() + "' name='" + guid + "table_" + field.Name + "' src='" + src + "' alt='" + title + "' title='" + title + "' /></a>";

                    }
                    else
                        return string.Empty;
                }
                else
                {
                    string url = System.Web.HttpContext.Current.Request.ApplicationPath;

                    View view = (View)field.View;

                    title = Map.Database.Localizer.Translate("Download") + " " + field.DisplayName;
                    if (!url.EndsWith("/"))
                        url += "/";
                    url += view.Controller + "/";
                    url += "Download/";
                    url += view.Name;
                    url += string.Format("?fieldName={0}&filename={1}", System.Web.HttpContext.Current.Server.UrlEncode(field.Name), System.Web.HttpContext.Current.Server.UrlEncode(value.ToString().Replace("\"", @"\" + "\"")));
                    url += "&pk=" + pk;

                    string href = url;
                    return "<a target='durados_document' href=\"" + href + "\" alt='" + title + "' title='" + title + "'>" + value.ToString() + "</a>"; // + value.ToString() + 
                }

            }
            else if (field.FtpUpload != null)
            {
                if (field.FtpUpload.UploadFileType == UploadFileType.Image)
                {
                    string width = string.Empty;
                    string height = string.Empty;

                    if (field.FtpUpload.Width > 0)
                    {
                        width = "width:" + field.FtpUpload.Width + "px;";
                    }
                    if (field.FtpUpload.Height > 0)
                    {
                        height = "height:" + field.FtpUpload.Height + "px;";
                    }

                    if (!string.IsNullOrEmpty(field.FtpUpload.DirectoryVirtualPath))
                    {
                        src = field.FtpUpload.DirectoryVirtualPath.Trim('/') + "/" + value.ToString().TrimStart('/');

                        title = field.FtpUpload.Title;
                        if (title == null) title = string.Empty;


                        string filename = value.ToString();
                        string[] segments = filename.Split('.');
                        string extension = segments.Length > 0 ? segments[segments.Length - 1] : string.Empty;

                        string href = src;

                        if (!imageExtensions.Contains(extension.ToLower()))
                        {
                            src = System.Web.HttpContext.Current.Request.ApplicationPath + "/Content/Images/file.gif";
                        }

                        /**imgLoadSelector (jQuery+css onload+display image selector solution) solution for image onload, onload is used here for displaying image when finishing loading, it doesn't work for Firefox*/
                        return "<a href='" + href + "'><img preview='1' dialog='1' format='" + format + "' " + onload + " style='display:none;" + width + height + "' class='" + field.CssClass + " imgLoadSelector' border=0 colName='" + field.Name + "' id='" + guid + "table_" + field.Name.ReplaceNonAlphaNumeric() + "' name='" + guid + "table_" + field.Name + "' src='" + src + "' alt='" + title + "' title='" + title + "' /></a>";
                    }
                    else
                    {
                        if (field.FtpUpload.StorageType != StorageType.Ftp)
                        {
                            string url = value.ToString();
                            title = Map.Database.Localizer.Translate("Download") + " " + field.DisplayName;

                            string href = url;
                            /**imgLoadSelector (jQuery+css onload+display image selector solution) solution for image onload, onload is used here for displaying image when finishing loading, it doesn't work for Firefox*/
                            return "<a target='durados_document' href=\"" + href + "\" alt='" + title + "' title='" + title + "'><img fullUrl='yes' preview='1' dialog='1' format='" + format + "' " + onload + " style='display:none;" + width + height + "' class='" + field.CssClass + " imgLoadSelector' border=0 colName='" + field.Name + "' id='" + guid + "table_" + field.Name.ReplaceNonAlphaNumeric() + "' name='" + guid + "table_" + field.Name + "' src='" + href + "' alt='" + title + "' title='" + title + "' /></a>"; // + value.ToString() + 
 
                        }
                        else
                        {
                            string url = System.Web.HttpContext.Current.Request.ApplicationPath;

                            title = Map.Database.Localizer.Translate("Download") + " " + field.DisplayName;

                            View view = (View)field.View;

                            if (!url.EndsWith("/"))
                                url += "/";
                            url += view.Controller + "/";
                            url += "Download/";
                            url += view.Name;
                            url += string.Format("?fieldName={0}&filename={1}", System.Web.HttpContext.Current.Server.UrlEncode(field.Name), System.Web.HttpContext.Current.Server.UrlEncode(value.ToString().Replace("\"", @"\" + "\"")));
                            url += "&pk=" + pk;

                            string href = url;
                            /**imgLoadSelector (jQuery+css onload+display image selector solution) solution for image onload, onload is used here for displaying image when finishing loading, it doesn't work for Firefox*/
                            return "<a target='durados_document' href=\"" + href + "\" alt='" + title + "' title='" + title + "'><img preview='1' dialog='1' format='" + format + "' " + onload + " style='display:none;" + width + height + "' class='" + field.CssClass + " imgLoadSelector' border=0 colName='" + field.Name + "' id='" + guid + "table_" + field.Name.ReplaceNonAlphaNumeric() + "' name='" + guid + "table_" + field.Name + "' src='" + href + "' alt='" + title + "' title='" + title + "' /></a>"; // + value.ToString() + 
                        }
                    }
                }
                else
                {
                    string url = System.Web.HttpContext.Current.Request.ApplicationPath;
                    string keyName = value.ToString();
            
                    title = Map.Database.Localizer.Translate("Download") + " " + field.DisplayName;

                    View view = (View)field.View;

                    if (field.FtpUpload.StorageType == StorageType.Ftp)
                    {
                        if (!url.EndsWith("/"))
                            url += "/";
                        url += view.Controller + "/";
                        url += "Download/";
                        url += view.Name;
                        url += string.Format("?fieldName={0}&filename={1}", System.Web.HttpContext.Current.Server.UrlEncode(field.Name), System.Web.HttpContext.Current.Server.UrlEncode(value.ToString().Replace("\"", @"\" + "\"")));
                        url += "&pk=" + pk;
                    }
                    else if (field.FtpUpload.StorageType == StorageType.Aws)
                    {
                        url = GetAwsDownloadUrl(field, pk, keyName);
                    }
                    else if (field.FtpUpload.StorageType == StorageType.Azure)
                    {
                        url = GetAzureDownloadUrl(field, pk, keyName);
                    }

                    string href = url;
                    return "<a target='durados_document' href=\"" + href + "\" alt='" + title + "' title='" + title + "'>" + value.ToString() + "</a>"; // + value.ToString() + 
                }
            }
            else
            {
                string url = value.ToString();
                title = Map.Database.Localizer.Translate("Download") + " " + field.DisplayName;

                string href = url;
                /**imgLoadSelector (jQuery+css onload+display image selector solution) solution for image onload, onload is used here for displaying image when finishing loading, it doesn't work for Firefox*/
                return "<a target='durados_document' href=\"" + href + "\" alt='" + title + "' title='" + title + "'><img fullUrl='yes' preview='1' dialog='1' format='" + format + "' " + onload + " style='display:none;' class='" + field.CssClass + " imgLoadSelector' border=0 colName='" + field.Name + "' id='" + guid + "table_" + field.Name.ReplaceNonAlphaNumeric() + "' name='" + guid + "table_" + field.Name + "' src='" + href + "' alt='" + title + "' title='" + title + "' /></a>"; // + value.ToString() + 

            }


        }




        private string GetDownloadIcon(ColumnField field, string pk, string prefix, string guid)
        {



            string title = Map.Database.Localizer.Translate("View Document");

            string href = GetDownloadUrl(field, pk);
            string src = General.GetRootPath() + "Content/Images/ViewUpload.gif";
            return "<a target='durados_document' id='" + guid + prefix + "DownloadIcon_" + field.Name.ReplaceNonAlphaNumeric() + "' d_href='" + href + "' href='" + href + "' class='uploadIcon'><img src='" + src + "' title='" + title + "' alt='" + title + "'></a>"; // + value.ToString() + 
        }


        public virtual string GetDownloadUrl(ColumnField field, string pk)
        {
            string actionName = string.Empty;


            if (field.Upload != null)
            {
                actionName = "Download";

            }
            else if (field.FtpUpload != null)
            {
                string keyName = "__filename__";
                actionName = "Download";
                if (field.FtpUpload.StorageType == StorageType.Aws)
                {
                    return GetAwsDownloadUrl(field, pk, keyName);
                }
                else if (field.FtpUpload.StorageType == StorageType.Azure)
                {
                    return GetAzureDownloadUrl(field, pk, keyName);
                }
            }
            else
            {
                throw new UploadPropertyMissingException();
            }


            string url = System.Web.HttpContext.Current.Request.ApplicationPath;

            if (!url.EndsWith("/")) url += "/";

            View view = (View)field.View;

            url += view.Controller + "/" + actionName + "/" + view.Name;

            url += string.Format("?fieldName={0}&filename={1}", System.Web.HttpContext.Current.Server.UrlEncode(field.Name), "__filename__");

            url += "&pk=" + pk;

            return url;
        }

        private string GetAzureDownloadUrl(ColumnField field, string pk, string keyName)
        {
            string seperator = "/";
            if (string.IsNullOrEmpty(field.FtpUpload.DirectoryVirtualPath))
            {
                string container = field.FtpUpload.DirectoryBasePath.Trim(seperator.ToCharArray()) + seperator;
                return string.Format(Maps.AzureStorageUrl + "{2}", field.FtpUpload.AzureAccountKey, container, keyName);
            }
            else
            {
                return field.FtpUpload.DirectoryVirtualPath.Trim(seperator.ToCharArray()) + seperator + keyName;
            }
        }

        private string GetAwsDownloadUrl(ColumnField field, string pk, string keyName)
        {
            string seperator = "/";
            if (string.IsNullOrEmpty(field.FtpUpload.DirectoryVirtualPath))
            {
                string bucketName = field.FtpUpload.DirectoryBasePath.Trim(seperator.ToCharArray()) + seperator;
                return string.Format("http://s3.amazonaws.com/{0}{1}", bucketName, keyName);
            }
            else
            {
                return field.FtpUpload.DirectoryVirtualPath.Trim(seperator.ToCharArray()) + seperator + keyName;
            }
        }

        private string GetDatabaseSrc()
        {
            throw new NotImplementedException();
        }

        protected virtual string GetCheckForTableView(ColumnField field, object value, string guid)
        {
            //string attributes = string.Empty;
            //if (value == null || value == DBNull.Value)
            //    return GetCheck(field, "table_", false, false, true, string.Empty, guid, attributes);
            //else
            //    return GetCheck(field, "table_", Convert.ToBoolean(value), true, true, string.Empty, guid, attributes);

            bool check = value != null && value != DBNull.Value && Convert.ToBoolean(value);
            string v = check ? "&#8730;" : "&nbsp;";
            string className = check ? "checked" : "not-checked";
            return "<span class='" + className + "'>" + v + "</span>";

        }

        protected virtual string GetCheckForEdit(ColumnField field, string pk, string value, string guid)
        {
            object o = field.ConvertFromString(value);
            bool b = false;
            if (o is bool)
                b = (bool)o;
            string attr = GetAdminPreviewAttr(field);
            return GetCheck(field, editPrefix, b, true, field.IsDisableForEdit(guid), pk, guid, attr);
        }

        protected virtual string GetRadioForEdit(ColumnField field, string pk, string value, string guid)
        {
            object o = field.ConvertFromString(value);
            bool b = false;
            if (o is bool)
                b = (bool)o;
            return GetRadio(field, editPrefix, b, true, field.IsDisableForEdit(guid), field.RadioOrientation, pk, guid);
        }

        protected virtual string GetCheckForCreate(ColumnField field, string pk, string guid)
        {
            return GetCheck(field, createPrefix, field.ConvertDefaultToBool(), true, field.IsDisableForCreate(), pk, guid, string.Empty);
        }

        protected virtual string GetCheckForReport(ColumnField field, string pk, string guid)
        {
            return GetCheck(field, createPrefix, field.ConvertDefaultToBool(), true, false, pk, guid, string.Empty);
        }

        protected virtual string GetRadioForCreate(ColumnField field, string pk, string guid)
        {
            return GetRadio(field, createPrefix, field.ConvertDefaultToBool(), true, field.IsDisableForCreate(), field.RadioOrientation, pk, guid);
        }

        //protected virtual string GetCheck(ColumnField field, string prefix, bool value)
        //{
        //    return GetCheck(field, prefix, value, true, false);
        //}

        protected virtual string GetCheck(ColumnField field, string prefix, bool value, bool visible, bool disabled, string pk, string guid, string attributes)
        {
            return "<input type='checkbox' safari='1' colName='" + field.Name + "' id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "' " + (value ? "checked='checked'" : "") + (visible ? "" : "style='visibility:hidden' ") + GetDisabledHtmlAttribute(disabled) + "  viewName='" + field.View.Name + "' pk='" + pk + "' " + attributes + " />";

        }





        protected virtual string GetRadio(ColumnField field, string prefix, bool value, bool visible, bool disabled, Orientation orientation, string pk, string guid)
        {
            return "<div radioButtons='radioButtons' id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "'  viewName='" + field.View.Name + "' pk='" + pk + "' style='" + GetOrientationStyle(orientation) + GetVisibilityStyle(visible) + "'>" + GetRadio(field, prefix, value, true, disabled, guid) + (orientation == Orientation.Vertical ? "<br>" : "") + GetRadio(field, prefix, value, false, disabled, guid) + "</div>";

        }

        protected virtual string GetRadio(ColumnField field, string prefix, bool value, bool b, bool disabled, string guid)
        {
            return "<input colName='" + field.Name + "' type='radio'  name='" + guid + prefix + field.Name + "' value='" + GetRadioValue(field, b) + "' " + (value ? "checked='checked'" : "") + GetDisabledHtmlAttribute(disabled) + " />" + GetRadioLabel(field, b);

        }

        protected virtual string GetCheckForFilter(ColumnField field, bool? value, string guid)
        {
            string style = GetFilterStyle(field);
            //return GetCheck("filter_", value);
            StringBuilder dropDown = new StringBuilder();
            dropDown.Append("<select " + style + " id='" + guid + filterPrefix + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "' >");

            dropDown.Append("<option value=''></option>");

            dropDown.Append("<option value='False' " + ((value.HasValue && !value.Value) ? "selected='selected'" : "").ToString() + ">N</option>");
            dropDown.Append("<option value='True' " + ((value.HasValue && value.Value) ? "selected='selected'" : "").ToString() + ">Y</option>");


            dropDown.Append("</select>");

            return dropDown.ToString();
        }

        public override string GetElementForInlineAdding(Field field, string guid)
        {
            return GetElementForCreate((ColumnField)field, guid).Replace(createPrefix, inlineAddingPrefix);
        }

        public override string GetElementForInlineEditing(Field field, string guid)
        {
            return GetElementForEdit((ColumnField)field, guid).Replace(editPrefix, inlineEditingPrefix);
        }

        public override string GetElementForCreate(Field field, string guid)
        {
            return GetElementForCreate(field, string.Empty, string.Empty, guid);
        }

        public override string GetElementForCreate(Field field, string pk, string value, string guid)
        {
            return GetElementForCreate((ColumnField)field, pk, value, guid);
        }

        public virtual string GetElementForCreate(ColumnField field, string pk, string value, string guid)
        {
            switch (GetHtmlControlType(field))
            {
                case HtmlControlType.Hidden:
                    return GetHiddenForCreate(field, pk, guid);
                case HtmlControlType.Check:
                    return GetCheckForCreate(field, pk, guid);
                case HtmlControlType.Radio:
                    return GetRadioForCreate(field, pk, guid);
                case HtmlControlType.Upload:
                    return GetUploadForCreate(field, pk, guid);
                case HtmlControlType.DropDown:
                    return GetEnumForCreate(field, pk, guid);
                case HtmlControlType.TextArea:
                    return GetTextAreaForCreate(field, pk, guid);
                case HtmlControlType.AutocompleteColumn:
                    return GetAutocompleteForCreate(field, pk, guid);
                case HtmlControlType.Url:
                    return GetUrlForCreate(field, pk, guid);
                case HtmlControlType.ColorPicker:
                    return GetColorPickerForCreate(field, pk, guid);
                case HtmlControlType.CheckList:
                    return GetCheckListForCreate(field, pk, value, guid);

                default:
                    return GetTextForCreate(field, pk, guid);
            }
        }

        public override string GetElementForReport(Field field, string guid)
        {
            return GetElementForReport(field, string.Empty, string.Empty, guid);
        }

        public override string GetElementForReport(Field field, string pk, string value, string guid)
        {
            return GetElementForReport((ColumnField)field, pk, value, guid);
        }

        public virtual string GetElementForReport(ColumnField field, string pk, string value, string guid)
        {
            switch (GetHtmlControlType(field))
            {
                case HtmlControlType.Hidden:
                    return GetHiddenForCreate(field, pk, guid);
                case HtmlControlType.Check:
                    return GetCheckForReport(field, pk, guid);
                case HtmlControlType.Radio:
                    return GetRadioForCreate(field, pk, guid);
                case HtmlControlType.Upload:
                    return GetUploadForCreate(field, pk, guid);
                case HtmlControlType.DropDown:
                    return GetEnumForCreate(field, pk, guid);
                case HtmlControlType.TextArea:
                    return GetTextAreaForCreate(field, pk, guid);
                case HtmlControlType.AutocompleteColumn:
                    return GetAutocompleteForReport(field, pk, guid);
                case HtmlControlType.Url:
                    return GetUrlForCreate(field, pk, guid);
                case HtmlControlType.CheckList:
                    return GetCheckListForCreate(field, pk, value, guid);

                default:
                    return GetTextForReport(field, pk, guid);
            }
        }

        public override string GetElementForEdit(Field field, string guid)
        {
            return GetElementForEdit(field, string.Empty, string.Empty, guid);
        }

        public override string GetElementForEdit(Field field, DataRow dataRow, string guid)
        {
            string pk = field.View.GetPkValue(dataRow);
            string value = field.GetValue(dataRow);

            return GetElementForEdit(field, pk, value, guid);
        }

        public override string GetElementForEdit(Field field, string pk, string value, string guid)
        {
            return GetElementForEdit((ColumnField)field, pk, value, guid);
        }

        public virtual string GetElementForEdit(ColumnField field, string pk, string value, string guid)
        {
            switch (GetHtmlControlType(field))
            {
                case HtmlControlType.Hidden:
                    return GetHiddenForEdit(field, pk, value, guid);
                case HtmlControlType.Check:
                    return GetCheckForEdit(field, pk, value, guid);
                case HtmlControlType.Radio:
                    return GetRadioForEdit(field, pk, value, guid);
                case HtmlControlType.Upload:
                    return GetUploadForEdit(field, pk, guid);
                case HtmlControlType.DropDown:
                    return GetEnumForEdit(field, pk, guid);
                case HtmlControlType.TextArea:
                    return GetTextAreaForEdit(field, pk, value, guid);
                case HtmlControlType.AutocompleteColumn:
                    return GetAutocompleteForEdit(field, pk, value, guid);
                case HtmlControlType.Url:
                    return GetUrlForEdit(field, pk, value, guid);
                case HtmlControlType.ColorPicker:
                    return GetColorPickerForEdit(field, pk, value, guid);
                case HtmlControlType.CheckList:
                    return GetCheckListForEdit(field, pk, value, guid);

                default:
                    return GetTextForEdit(field, pk, value, guid);
            }
        }

        protected virtual string GetCheckListForEdit(ColumnField field, string pk, string value, string guid)
        {
            return GetCheckList(field, editPrefix, GetStyle(field), field.IsDisableForEdit(guid), pk, guid);
        }

        protected virtual string GetCheckListForCreate(ColumnField field, string pk, string value, string guid)
        {
            return GetCheckList(field, createPrefix, GetStyle(field), field.IsDisableForCreate(), pk, guid);
        }

        protected virtual string GetCheckList(ColumnField field, string prefix, string style, bool disabled, string pk, string guid)
        {
            StringBuilder selectList = new StringBuilder();

            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            selectList.Append("<select " + style + " " + GetDisabledHtmlAttribute(disabled) + " id='" + id + "' name='" + field.Name + "' viewName='" + field.View.Name + "' pk='" + pk + "' style='display:none' multiple='multiple' d_loaded='d_loaded' role='childrenCheckList' class='dropdownchecklist' d_minWidth='" + field.MinWidth.ToString() + "'>");


            IEnumerable<SelectListItem> selectListItems = field.GetSelectList();

            selectList.Append("<option value=''>(All)</option>");

            foreach (SelectListItem item in selectListItems)
            {
                selectList.Append("<option value='" + item.Value + "' " + (item.Selected ? "selected='selected'>" : ">").ToString() + item.Text + "</option>");
            }

            selectList.Append("</select>");

            return selectList.ToString();

        }

        private string GetEnumForEdit(ColumnField field, string pk, string guid)
        {
            return GetEnumForRowView(field, editPrefix, string.Empty, pk, guid);
        }

        private string GetEnumForCreate(ColumnField field, string pk, string guid)
        {
            return GetEnumForRowView(field, createPrefix, field.DefaultValue == null ? string.Empty : field.DefaultValue.ToString(), pk, guid);
        }

        public string GetUploadForCreate(ColumnField field, string pk, string guid)
        {
            return GetUploadForRowView(field, createPrefix, pk, field.IsDisableForCreate(), guid);
        }

        public string GetUploadForEdit(ColumnField field, string pk, string guid)
        {
            return GetUploadForRowView(field, editPrefix, pk, field.IsDisableForEdit(guid), guid);
        }

        private string GetEnumForRowView(ColumnField field, string prefix, string selectedValue, string pk, string guid)
        {
            if (field.IsImageList())
            {
                return GetImageList(field, prefix, GetStyle(field), selectedValue, false, pk, guid, string.Empty);
            }
            else
            {
                return GetDropDown(field, prefix, GetStyle(field), selectedValue, false, pk, guid, GetAdminPreviewAttr(field));
            }
        }

        private string GetImageList(ColumnField field, string prefix, string style, string selectedValue, bool disabled, string pk, string guid, string html)
        {

            StringBuilder ul = new StringBuilder();

            string insideTriggerAttributes = string.Empty;
            ul.Append("<div class='slider-container'>");
            //slider.Append("<ul class='ui-slider'>");//" id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() +
            ul.Append("<ul class='ui-slider'" + style + " name='" + field.Name + "'  viewName='" + field.View.Name + "' pk='" + pk + "' " + html + insideTriggerAttributes + " >");

            Dictionary<string, string> selectOptions = field.GetSelectOptions();


            foreach (string key in selectOptions.Keys)
            {
                //dropDown.Append("<li value='" + key.Trim('\'') + "' " + ((selectedValue == key) ? "selected='selected'" : "").ToString() + ">" + selectOptions[key] + "</li>");
                ul.Append("<li><div class='ui-slider-item'  value='" + key.Trim('\'') + "'  onclick='slider.UpdateSelectedImage(this);'>" + selectOptions[key] + "</div></li>");
            }

            ul.Append("</ul>");
            string hidden = string.Empty;
            if (prefix == createPrefix)
            {
                hidden = GetHiddenForCreate(field, pk, guid);
            }
            else
            {
                hidden = GetHiddenForEdit(field, pk, selectedValue, guid);
            }
            ul.Append(hidden);
            return ul.ToString();



        }

        protected virtual string GetDropDown(ColumnField field, string prefix, string style, string selectedValue, bool disabled, string pk, string guid, string html)
        {
            StringBuilder dropDown = new StringBuilder();

            bool isFilter = prefix == filterPrefix;


            string insideTriggerAttributes = string.Empty;


            if (!isFilter && field.TextHtmlControlType == TextHtmlControlType.DependencyCustom)
            {
                insideTriggerAttributes = " hasInsideDefault='hasInsideDefault' triggerName='' dependencyFieldName='' dependencyViewName='" + field.Name + "' insideDependency='insideDependency' dependentFieldViewName='" + field.View.Name + "' dependentFieldNames='" + field.Name + "' onchange='insideDependencyChange(this, \"" + guid + "\")' ";

            }



            dropDown.Append("<select " + style + " " + GetDisabledHtmlAttribute(disabled) + " id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "'  viewName='" + field.View.Name + "' pk='" + pk + "' " + html + insideTriggerAttributes + " >");

            Dictionary<string, string> selectOptions = field.GetSelectOptions();

            dropDown.Append("<option value=''></option>");

            foreach (string key in selectOptions.Keys)
            {
                dropDown.Append("<option value='" + key.Trim('\'') + "' " + ((selectedValue == key) ? "selected='selected'" : "").ToString() + ">" + selectOptions[key] + "</option>");
            }

            dropDown.Append("</select>");

            return dropDown.ToString();

        }



        private string GetUploadForRowView(ColumnField field, string prefix, string pk, bool disabled, string guid)
        {
            string fullUrl = string.Empty;
            bool isFullUrl = false;

            if (disabled)
            {
                return "<input upload='upload' disabled='disabled' type='text' id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "' />";
            }
            else
            {
                string width = string.Empty;
                string height = string.Empty;

                string uploadPath = string.Empty;

                string uploadAction = string.Empty;

                string src = string.Empty;


                if (field.Upload != null)
                {
                    uploadPath = field.Upload.GetFixedVirtualPath();
                    if (field.Upload.UploadFileType == UploadFileType.Image)
                    {
                        if (Maps.MultiTenancy)
                        {
                            src = GetDownloadUrl(field, pk);
                            uploadPath = src;
                        }
                        else
                        {
                            src = field.Upload.UploadVirtualPath + field.Name;
                        }
                    }

                }
                else if (field.FtpUpload != null)
                {
                    if (field.FtpUpload.UploadFileType == UploadFileType.Image)
                    {
                        if (field.FtpUpload.Width > 0)
                        {
                            width = "width:" + field.FtpUpload.Width + "px;";
                        }
                        if (field.FtpUpload.Height > 0)
                        {
                            height = "height:" + field.FtpUpload.Height + "px;";
                        }

                        if (!string.IsNullOrEmpty(field.FtpUpload.DirectoryVirtualPath))
                        {
                            src = field.FtpUpload.DirectoryVirtualPath.Trim('/') + "/";
                            uploadPath = src;
                        }
                        else
                        {
                            if (field.FtpUpload.StorageType == StorageType.Ftp)
                            {
                                src = GetDownloadUrl(field, pk);
                                uploadPath = src;
                            }
                            else
                            {
                                uploadPath = string.Empty;

                                fullUrl = " fullUrl='yes' ";
                                isFullUrl = true;
                            }
                        }
                    }
                    else
                    {
                    }
                }
                else
                {
                    uploadPath = string.Empty;

                    fullUrl = " fullUrl='yes' ";
                    isFullUrl = true;
                    return GetTextArea(field, prefix, disabled, string.Empty, pk, guid);
                }

                if (!string.IsNullOrEmpty(src) || isFullUrl)
                {
                    return "<div class='uploadDiv' id='" + guid + prefix + "upload_" + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "' ><span class='gbutton' id='" + guid + prefix + "upload_span_" + field.Name + "'>Upload</span><img style='visibility:hidden;" + width + height + "' id='" + guid + prefix + "upload_img_" + field.Name.ReplaceNonAlphaNumeric() + "' class='" + field.CssClass + "' src='" + src + "' title='" + field.DisplayName + "' alt='" + field.DisplayName + "' style='visibility:hidden;' UploadPath='" + uploadPath + "' />" + GetText(field, prefix, " upload='upload' " + fullUrl, string.Empty, field.CssClass, false, string.Empty, pk, guid) + "</div>" + GetDeleteUploadIcon(field, guid + prefix + field.Name, guid, prefix);
                }
                else
                {
                    return "<table><tr><td><div class='uploadDiv' id='" + guid + prefix + "upload_" + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "' ><span class='gbutton' id='" + guid + prefix + "upload_span_" + field.Name.ReplaceNonAlphaNumeric() + "'>Upload</span>" + GetText(field, prefix, " upload='upload' " + fullUrl, string.Empty, field.CssClass, false, string.Empty, pk, guid) + "</div></td><td>" + GetDeleteUploadIcon(field, guid + prefix + field.Name, guid, prefix) + "</td><td>" + GetDownloadIcon(field, pk, prefix, guid) + "</td></tr></table>";

                    //<script type=\"text/javascript\">$(document).ready(function() { setTimeout('UploadFile(\"" + field.Name + "\",\"" + prefix + "\",\"" + guid + "\")',1); });</script>";
                }
            }
        }


        private string GetDeleteUploadIcon(ColumnField field, string id, string guid, string prefix)
        {
            string title = Map.Database.Localizer.Translate("Clear Document");

            string url = System.Web.HttpContext.Current.Request.ApplicationPath;

            string action = "DeleteFile";

            //if (field.FtpUpload != null) action = "DeleteFtpFile";

            View view = (View)field.View;

            if (!url.EndsWith("/"))
                url += "/";
            url += view.Controller + "/" + action + "/" + view.Name;

            url += string.Format("?fieldName={0}", System.Web.HttpContext.Current.Server.UrlEncode(field.Name));

            url += "&filename=";

            string src = General.GetRootPath() + "Content/Images/Delete.png";
            return "<img id='" + guid + prefix + "DeleteIcon_" + field.Name.ReplaceNonAlphaNumeric() + "' class='fileClearImg' onclick='DeleteFile(\"" + url + "\", \"" + id + "\", \"" + field.Name + "\", \"" + guid + "\", \"" + prefix + "\")' src='" + src + "' title='" + title + "' alt='" + title + "'>"; // + value.ToString() + 

        }

        public override string GetElementForFilter(Field field, object value, string guid)
        {
            return GetElementForFilter((ColumnField)field, value, guid);
        }

        public virtual string GetElementForFilter(ColumnField field, object value, string guid)
        {
            bool? b = null;
            string style = GetFilterStyle(field);

            switch (GetHtmlControlType(field))
            {
                case HtmlControlType.Check:
                    if (value != null && value is string && ((string)value) != string.Empty)
                        b = Convert.ToBoolean(value);
                    return GetCheckForFilter(field, b, guid);
                case HtmlControlType.Radio:
                    if (value != null && value is string && ((string)value) != string.Empty)
                        b = Convert.ToBoolean(value);
                    return GetCheckForFilter(field, b, guid);
                case HtmlControlType.DropDown:
                    return GetDropDown(field, filterPrefix, style, (value ?? (field.DefaultValue ?? string.Empty)).ToString(), field.DisableInFilter, string.Empty, guid, " onkeypress='handleEnterFilter(this, event, \"" + guid + "\")' ");
                //case HtmlControlType.TextArea:
                //    return "";
                case HtmlControlType.AutocompleteColumn:
                    if (field.AutocompleteFilter)
                        return GetAutocomplete(field, filterPrefix, string.Empty, style, string.Empty, field.DisableInFilter, value == null ? string.Empty : value.ToString(), string.Empty, guid);
                    else
                        return GetTextForFilter(field, value == null ? string.Empty : value.ToString(), guid);
                default:
                    return GetTextForFilter(field, value == null ? string.Empty : value.ToString(), guid);
            }
        }

        protected virtual string GetHiddenForCreate(ColumnField field, string pk, string guid)
        {
            return GetHidden(field, createPrefix, string.Empty, field.CssClass, field.IsDisableForCreate(), field.ConvertDefaultToString(), pk, guid);
        }

        protected virtual string GetTextForCreate(ColumnField field, string pk, string guid)
        {
            return GetText(field, createPrefix, string.Empty, GetStyle(field), field.CssClass, field.IsDisableForCreate(), field.ConvertDefaultToString(), pk, guid);
        }

        protected virtual string GetTextForReport(ColumnField field, string pk, string guid)
        {
            return GetText(field, createPrefix, string.Empty, GetStyle(field), field.CssClass, false, field.ConvertDefaultToString(), pk, guid);
        }

        protected virtual string GetHiddenForEdit(ColumnField field, string pk, string value, string guid)
        {
            return GetHidden(field, editPrefix, string.Empty, field.CssClass, field.IsDisableForEdit(guid), value, pk, guid);
        }

        protected virtual string GetTextForEdit(ColumnField field, string pk, string value, string guid)
        {
            return GetText(field, editPrefix, string.Empty, GetStyle(field), field.CssClass, field.IsDisableForEdit(guid), value, pk, guid);
        }

        protected virtual string GetAutocompleteForCreate(ColumnField field, string value, string guid)
        {
            return GetAutocomplete(field, createPrefix, string.Empty, GetStyle(field), field.CssClass, field.IsDisableForCreate(), value, string.Empty, guid);
        }

        protected virtual string GetAutocompleteForReport(ColumnField field, string value, string guid)
        {
            return GetAutocomplete(field, createPrefix, string.Empty, GetStyle(field), field.CssClass, false, value, string.Empty, guid);
        }


        protected virtual string GetAutocompleteForEdit(ColumnField field, string pk, string value, string guid)
        {
            return GetAutocomplete(field, editPrefix, string.Empty, GetStyle(field), field.CssClass, field.IsDisableForEdit(guid), value, pk, guid);
        }

        protected virtual string GetColorPickerForCreate(ColumnField field, string value, string guid)
        {
            return GetColorPicker(field, createPrefix, string.Empty, string.Empty, field.CssClass, field.IsDisableForCreate(), value, string.Empty, guid);
        }

        protected virtual string GetColorPickerForEdit(ColumnField field, string pk, string value, string guid)
        {
            return GetColorPicker(field, editPrefix, string.Empty, GetStyle(field), field.CssClass, field.IsDisableForEdit(guid), value, pk, guid);
        }

        protected virtual string GetUrlForCreate(ColumnField field, string value, string guid)
        {
            return GetUrl(field, createPrefix, string.Empty, field.CssClass, field.IsDisableForCreate(), value, string.Empty, guid);
        }


        protected virtual string GetUrlForEdit(ColumnField field, string pk, string value, string guid)
        {
            return GetUrl(field, editPrefix, string.Empty, field.CssClass, field.IsDisableForEdit(guid), value, pk, guid);
        }

        protected virtual string GetUrl(ColumnField field, string prefix, string columnType, string cssClass, bool disabled, string value, string pk, string guid)
        {
            value = value ?? string.Empty;
            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            cssClass = string.IsNullOrEmpty(cssClass) ? "'" : " " + cssClass + "'";

            string[] values = new string[0];
            if (!string.IsNullOrEmpty(value))
            {
                values = value.Split('|');
            }


            string text = string.Empty;
            string target = "_blank";
            string href = "#";
            if (values.Length == 3)
            {
                text = values[0];
                target = values[1];
                href = values[2];
            }
            else if (values.Length == 1)
            {
                href = values[0];
            }

            if (string.IsNullOrEmpty(text) && !string.Equals(href, "#"))
            {
                text = href;
            }

            string className = "BatchEdit-icon";
            string title = Map.Database.Localizer.Translate("Batch Edit") + " " + field.GetLocalizedDisplayName();
            string onClick = "UrlDialog.Open(this, '" + (prefix == inlineEditingPrefix ? pk : string.Empty) + "', '" + field.Name + "', '" + field.DisplayName + "', '" + guid + "')";

            string element = string.Empty;
            if (!disabled)
            {
                element = GetInlineAddingImg(guid, title, className, onClick);
            }

            string urlInnerControl = string.Empty;
            string format = field.DisplayFormat.ToString();
            if (field.DisplayFormat == DisplayFormat.ButtonLink)
            {
                urlInnerControl = "<button type='button' class='button'>" + text + "</button>";
            }
            else
            {
                urlInnerControl = "<span>" + text + "</span>";
            }

            string url = "<span><a format='" + format + "' href='" + href + "' target='" + target + "' class='url" + cssClass + " id='" + id + "' name='" + field.Name + "' value='" + System.Web.HttpUtility.HtmlDecode(value) + "'" + " viewName='" + field.View.Name + "' pk='" + pk + "' d_type='Url' >" + urlInnerControl + "</a>" + element + "</span>";
           
            return url;
        }

        protected virtual string GetAutocomplete(ColumnField field, string prefix, string columnType, string style, string cssClass, bool disabled, string value, string pk, string guid)
        {
            value = value ?? string.Empty;
            //string autocomplete = "autocomplete=off";
            string id = guid + prefix + field.Name.ReplaceNonAlphaNumeric();
            cssClass = string.IsNullOrEmpty(cssClass) ? "'" : " " + cssClass + "'";
            string autocomplete = "<input " + style + " class='Autocomplete" + cssClass + " autocomplete='off' type='text' id='" + id + "' name='" + field.Name + "' value='" + System.Web.HttpUtility.HtmlDecode(value) + "'" + columnType + GetDisabledHtmlAttribute(disabled) + " viewName='" + field.View.Name + "' pk='" + pk + "' d_type='AutocompleteColumn' />";
            //return "<input " + GetDisabledHtmlAttribute(disabled) + " type='text' " + (string.IsNullOrEmpty(cssClass) ? "" : ("class='" + cssClass + "'")).ToString() + " id='" + guid + prefix + field.Name + "' name='" + field.Name + "'" + columnType + " value='" + System.Web.HttpUtility.HtmlDecode(value) + "' viewName='" + field.View.Name + "' pk='" + pk + "' " + autocomplete + " />";
            return autocomplete;
        }

        //protected virtual string GetTextForRowView(ColumnField field, string prefix)
        //{
        //    return GetText(field, prefix, value, "", field.CssClass);
        //}

        private string GetDisabledHtmlAttribute()
        {
            return "disabled='disabled'";
        }


        private string GetDisabledHtmlAttribute(bool disabled)
        {
            return ((disabled) ? GetDisabledHtmlAttribute() : string.Empty).ToString();
        }

        protected virtual string GetHidden(ColumnField field, string prefix, string columnType, string cssClass, bool disabled, string value, string pk, string guid)
        {
            value = value ?? string.Empty;
            return "<input " + GetDisabledHtmlAttribute(disabled) + " type='hidden' " + (string.IsNullOrEmpty(field.CssClass) ? "" : ("class='" + field.CssClass + "'")).ToString() + " id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "'" + columnType + " value='" + System.Web.HttpUtility.HtmlDecode(value) + "' viewName='" + field.View.Name + "' pk='" + pk + "' " + GetAdminPreviewAttr(field) + "/>";
        }

        protected virtual string GetColorPicker(ColumnField field, string prefix, string columnType, string style, string cssClass, bool disabled, string value, string pk, string guid)
        {
            StringBuilder html = new StringBuilder();
            bool hasValue = !string.IsNullOrEmpty(value);

            html.Append("<span class='colorPicker'>");
            html.Append("<input type='checkbox' title='" + (Map.Database.Localizer!= null ? Map.Database.Localizer.Translate("Use or do not use this color to override the skin color") : string.Empty) + "' safari='1' colName='" + field.Name + "' id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "_changeEnabled' " + (hasValue ? "checked='checked'" : "") + GetDisabledHtmlAttribute(disabled) + "  viewName='" + field.View.Name + "' pk='" + pk + "'/>");
            html.Append("<span class='colorPicker-override'></span>");
            html.Append("<input color='1' " + style + " " + GetDisabledHtmlAttribute(disabled) + " type='text' " + (string.IsNullOrEmpty(cssClass) ? "" : ("class='" + cssClass + "'")).ToString() + " id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "'" + columnType + " value='" + System.Web.HttpUtility.HtmlDecode(value) + "' viewName='" + field.View.Name + "' pk='" + pk + "' " + GetAdminPreviewAttr(field) + GetDisabledHtmlAttribute(disabled) + " />");
            html.Append("</span>");
            return html.ToString();
        }

        protected virtual string GetText(ColumnField field, string prefix, string columnType, string style, string cssClass, bool disabled, string value, string pk, string guid)
        {
            value = value ?? string.Empty;

            string autocomplete = "autocomplete=" + (field.BrowserAutocomplete ? "'on'" : "'off'");
            bool isDateField = field.GetColumnFieldType() == ColumnFieldType.DateTime;

            if (disabled && isDateField)
                cssClass = string.Empty;

            string dateFormatAttr = isDateField ? " df='" + field.Format + "'" : string.Empty;
            string dateTypeAttr = isDateField ? " dt='" + (int)field.DisplayFormat + "'" : string.Empty;
            
            string inputType = "text";
            if (Durados.Web.Infrastructure.General.IsMobile() && isDateField)
            {
                inputType = "date";
            }
            string html = string.Empty;
            string htmlTypeAtt = "";

            if (field.SpecialColumn.Equals(SpecialColumn.Password))
            {
                inputType = "password";
            }
            else if (field.SpecialColumn.Equals(SpecialColumn.Html))
            {
                html = "<div" + " id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "htmlType" + "' name='" + field.Name + "htmlType'" + ">" + value + "</div>";
                inputType = "hidden";
                htmlTypeAtt = " htmlType='htmlType' ";
            }

            return html + "<input  " + htmlTypeAtt + style + " " + GetDisabledHtmlAttribute(disabled) + " type='" + inputType + "' " + (string.IsNullOrEmpty(cssClass) ? "" : ("class='" + cssClass + "'")).ToString() + " id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "'" + columnType + " value='" + System.Web.HttpUtility.HtmlDecode(value) + "' viewName='" + field.View.Name + "' pk='" + pk + "' " + autocomplete + dateFormatAttr + dateTypeAttr + GetAdminPreviewAttr(field) + GetDictionaryAttributes(field) + " />";
        }

        private string getComparerString(string str, string prefix, string suffix)
        {
            int beginPos = str.IndexOf(prefix, 0);
            string value = "=";

            if (beginPos > -1)
            {
                int start = beginPos + prefix.Length;
                int end = str.IndexOf(suffix, start);
                if (end > -1)
                {
                    value = str.Substring(start, end - start);
                }
            }

            return value;
        }

        public static string WrapToken(string str, string prefix, string suffix)
        {
            int i = 0;
            while (i < str.Length)
            {
                if (char.IsDigit(str[i]))
                {
                    break;
                }
                i++;
            }

            if (i == 0) // is only digit
            {
                return prefix + str + suffix;
            }

            var first = str.Substring(0, i);
            var second = str.Substring(i, str.Length - i);
            return prefix + first + suffix + second;
        }

        private string GetComparerTranslator(string comparer, AdvancedFilterType filterType)
        {
            string displayedComparer = comparer;
            IDictionary<string, string> comparersTranslations = null;

            if (filterType == AdvancedFilterType.TextFilter)
            {
                comparersTranslations = Durados.DataAccess.Filter.StringFilterComparisons;
            }
            else
            {
                comparersTranslations = Durados.DataAccess.Filter.MathematicsFilterComparisons;
            }

            if (comparersTranslations.ContainsKey(comparer))
            {
                string comparerTranslatorKey = comparersTranslations[comparer];
                displayedComparer = Map.Database.Localizer.Translate(comparerTranslatorKey);
            }

            return displayedComparer;
        }

        protected virtual string GetTextForFilter(ColumnField field, string value, string guid)
        {
            string style = GetFilterStyle(field);
            AdvancedFilterType filterType = AdvancedFilterType.TextFilter;

            //Init filter type
            if (field.AdvancedFilter)
            {
                if (field.IsNumeric)
                {
                    filterType = AdvancedFilterType.NumericFilter;
                }
                else if (field.GetColumnFieldType() == ColumnFieldType.DateTime)
                {
                    filterType = AdvancedFilterType.DateFilter;
                }
                else
                {
                    if (field.IsMilestonesField)
                    {
                        filterType = AdvancedFilterType.DateFilter;
                    }
                }
            }

            string filterTypeAttr = "filterType='" + filterType.ToString() + "'";
            string token = Durados.DataAccess.Filter.TOKEN;
            string comparer = getComparerString(value, token, token);
            string comparerWrapper = WrapToken(comparer, token, token);
            string displayedComparer = comparer;
            string displayedText = string.Empty;
            string tooltip = string.Empty;

            displayedComparer = GetComparerTranslator(comparer, filterType);
            if (filterType == AdvancedFilterType.TextFilter)
            {
                displayedText = value.Replace(comparerWrapper, displayedComparer);
                tooltip = displayedText;
            }
            else
            {
                tooltip = value.Replace(comparerWrapper, displayedComparer);
                if (comparer == "between")
                {
                    string toComparer = "To";
                    string toComparerWrapper = WrapToken(toComparer, token, token);
                    string toComparerTranslation = Map.Database.Localizer.Translate(toComparer);

                    displayedText = value.Replace(toComparerWrapper, toComparerTranslation);
                    tooltip = tooltip.Replace(toComparerWrapper, toComparerTranslation);

                    displayedText = displayedText.Replace(comparerWrapper, displayedComparer);
                }
                else if (comparer.Contains("empty"))
                {
                    displayedText = value.Replace(comparerWrapper, displayedComparer);
                }
                else
                {
                    displayedText = value.Replace(comparerWrapper, comparer);
                }
            }

            string attributes = (field.IsMilestonesField ? " df='" + field.Format + "'" : "") + " onfocus='Durados.dropdowndiv.show(this" + (field.IsMilestonesField ? ", true" : "") + ")' " + filterTypeAttr + " d_val='" + value + "' columnType='column' title='" + tooltip + "' onkeydown='handleEnterFilter(this, event, \"" + guid + "\")' ";

            return GetText(field, filterPrefix, attributes, style, "advancedFilter", false, displayedText, string.Empty, guid);
        }

        protected virtual string GetTextAreaForEdit(ColumnField field, string pk, string value, string guid)
        {
            return GetTextArea(field, editPrefix, field.IsDisableForEdit(guid), value, pk, guid);
        }

        protected virtual string GetTextAreaForCreate(ColumnField field, string pk, string guid)
        {
            return GetTextArea(field, createPrefix, field.IsDisableForCreate(), field.ConvertDefaultToString(), pk, guid);
        }

        protected virtual string GetTextArea(ColumnField field, string prefix, bool disabled, string value, string pk, string guid)
        {
            if (disabled)
            {
                return "<div class='" + field.CssClass + "' id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "' viewName='" + field.View.Name + "' pk='" + pk + "'>" + value + "</div>";
            }
            else
            {
                string upload = field.Upload == null && field.FtpUpload == null ? "" : "upload='upload' guid='" + guid + "'";
                string css = string.Empty;
                string rich = string.Empty;
                if (!Durados.Web.Infrastructure.General.IsMobile())
                {
                    css = string.IsNullOrEmpty(field.CssClass) ? "wtextarea" : field.CssClass;
                    rich = field.Rich.ToString().ToLower();
                }
                else
                {
                    css = "wtextareamobile";
                    rich = "false";
                }
                return "<textarea class='" + css + "' rich='" + rich + "' id='" + guid + prefix + field.Name.ReplaceNonAlphaNumeric() + "' name='" + field.Name + "' viewName='" + field.View.Name + "' pk='" + pk + "' " + upload + GetAdminPreviewAttr(field) + " " + GetDictionaryAttributes(field) + " >" + value + "</textarea>";
            }
        }

        public override string GetValidationElements(Field field, DataAction dataAction, string guid)
        {
            if (dataAction == DataAction.Create && field.IsDisableForCreate())
                return string.Empty;

            if (dataAction == DataAction.Edit && field.IsDisableForEdit(guid))
                return string.Empty;

            string validationElements = string.Empty;

            validationElements += GetRequiredValidationElement(field, guid);
            validationElements += GetValidationElement(field, guid);

            return validationElements;
        }

        private string GetDictionaryAttributes(ColumnField field)
        {
            StringBuilder sb = new StringBuilder(string.Empty);

            if (!string.IsNullOrEmpty(field.DictionaryViewFieldName))
                sb.AppendFormat("dictionaryView='{0}' ", field.DictionaryViewFieldName);
            if (field.DictionaryType != null)
                sb.AppendFormat(" dicType='{0}'", field.DictionaryType.ToString());

            return sb.ToString();
        }

    }

    public class UploadPropertyMissingException : DuradosException
    {
        public UploadPropertyMissingException()
            : base("The config file upload property is missing.")
        {
        }
    }
}
