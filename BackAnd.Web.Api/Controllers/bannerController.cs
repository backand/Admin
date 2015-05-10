using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace BackAnd.Web.Api.Controllers
{
    public class bannerController : apiController
    {
        public HttpResponseMessage Get()
        {
            try
            {
                string style = @"<style>nav#backand-banner {margin-bottom: 0;} #backand-banner .container {width: 100%;}#backand-banner .navbar-nav > li > a {padding-top:0;padding-bottom:0;color: #4c4c4c;}#backand-banner .navbar-brand{padding-top:0;}#backand-banner .navbar-nav > li > a {padding-top:5px !important; padding-bottom:5px !important;}#backand-banner.navbar {min-height:20px !important;background: #f3b858;border-bottom: 1px solid #1ba085;}#backand-banner .navbar-brand {padding-top: 5px;padding-bottom: 0;color: #4c4c4c;} #backand-banner .dropdown-toggle{cursor:pointer;} </style>";
                string banner = @"<nav id=""backand-banner"" class=""navbar navbar-default"" role=""navigation""><div class=""container""><div class=""navbar-header""><button type=""button"" class=""navbar-toggle"" data-toggle=""collapse"" data-target=""#bs-example-navbar-collapse-1""><span class=""sr-only"">Toggle navigation</span><span class=""icon-bar""></span><span class=""icon-bar""></span><span class=""icon-bar""></span></button><a class=""navbar-brand"" href=""https://www.backand.com/"">Back&</a></div><div class=""collapse navbar-collapse"" id=""bs-example-navbar-collapse-1""><ul class=""nav navbar-nav""><li><a href=""[adminUrl]/Admin/Pages?menuId="" target=""_bkadmin"">Pages</a></li></ul><ul class=""nav navbar-nav navbar-right""><li class=""dropdown""><a data-toggle=""dropdown"" class=""dropdown-toggle backand-username"">[username] <b class=""caret""></b></a><ul class=""dropdown-menu""><li><a href=""[adminUrl]/Account/ChangePassword"" target=""_bkadmin"">Change Password</a></li><li><a href=""https://www.backand.com/apps"" target=""_bkadmin"">My Consoles</a></li><li><a href=""http://blog.backand.com/questions"" target=""_bkadmin"">Support</a></li></ul></li></ul><ul class=""nav navbar-nav navbar-right""><li class=""dropdown""><a data-toggle=""dropdown"" class=""dropdown-toggle"">Admin <b class=""caret""></b></a><ul class=""dropdown-menu""><li><a href=""[adminUrl]/Admin/Index/Database"" target=""_bkadmin"">Default Settings</a></li><li><a href=""[adminUrl]/Admin/Index/View"" target=""_bkadmin"">Tables &amp; Views</a></li><li><a href=""[adminUrl]/Admin/Index/Workspace"" target=""_bkadmin"">Workspaces</a></li><li><a href=""[adminUrl]/Home/IndexPage/v_durados_User"" target=""_bkadmin"">Users</a></li><li><a href=""[adminUrl]/Admin/Index/Rule"" target=""_bkadmin"">Business Rules</a></li><li><a href=""[adminUrl]/Home/Index/Durados_Log"" target=""_bkadmin"">Trace</a></li></ul></li></ul></div></div></nav>";
                string responseBody = @" var adminInfo = null; var openAdmin = function(newPage){window.open(adminInfo.url)}; $(document).ready(function () { document.addEventListener(""onlogin"", function (e) { if (!adminInfo) adminInfo = backand.security.banner.getAdminInfo(); if (!adminInfo) return; $('body').remove('.backand-banner');  $('body').prepend('" + style + banner + @"'); $('#backand-banner .backand-username').html($('#backand-banner .backand-username').html().replace('[username]', adminInfo.username)); $('#backand-banner a').each(function(){if($(this).attr('href') && $(this).attr('href') != '') $(this).attr('href', $(this).attr('href').replace('[adminUrl]',adminInfo.url));}); }); }); ";

                HttpResponseMessage response = Request.CreateResponse(System.Net.HttpStatusCode.OK, responseBody, new TextPlainFormatter());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/javascript");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.Name = "backandbanner";
                response.Content.Headers.ContentDisposition.FileName = "backandbanner.js";

                return response;
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        
        [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
        public IHttpActionResult Post()
        {
            try
            {
                if (IsAdmin())
                    return Ok(new { url = Durados.Web.Mvc.UI.Helpers.RestHelper.GetCurrentAdminAppUrl(), username = Durados.Web.Mvc.UI.Helpers.RestHelper.GetCurrentUsername() });// System.Web.HttpContext.Current.Items["appname"].ToString();

                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        
    }

    
}
