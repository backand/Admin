using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Threading;
using System.Web.Http.WebHost;
using System.Web;
using System.Web.Http.Hosting;

namespace BackAnd.Web.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "uploadFile",
                routeTemplate: "1/file/{provider}",
                defaults: new { controller = "file", action = "putObject" }
            );

            config.Routes.MapHttpRoute(
                name: "dashboardData",
                routeTemplate: "1/dashboard/data/{id}",
                defaults: new { controller = "dashboardData" }
            );

            config.Routes.MapHttpRoute(
                name: "dashboardConfig",
                routeTemplate: "1/dashboard/config/{id}",
                defaults: new { controller = "dashboardConfig", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "contentConfig",
                routeTemplate: "1/content/config/{id}",
                defaults: new { controller = "contentConfig", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "chartData",
                routeTemplate: "1/chart/data/{id}",
                defaults: new { controller = "chartData" }
            );

            config.Routes.MapHttpRoute(
                name: "chartConfig",
                routeTemplate: "1/chart/config/{id}",
                defaults: new { controller = "chartConfig", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "viewDataWithId",
                routeTemplate: "1/view/data/{name}/{id}",
                defaults: new { controller = "viewData", name = RouteParameter.Optional, id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "cloudServiceProviderWithId",
                routeTemplate: "admin/function/provider/{id}",
                defaults: new { controller = "viewData", name = "cloudServiceProvider", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
               name: "tableDataWithId",
               routeTemplate: "1/table/data/{name}/{id}",
               defaults: new { controller = "viewData", name = RouteParameter.Optional, id = RouteParameter.Optional }
           );


            config.Routes.MapHttpRoute(
               name: "objectWithIdAndCollection",
               routeTemplate: "1/objects/{name}/{id}/{collection}",
               defaults: new { controller = "viewData", name = RouteParameter.Optional, id = RouteParameter.Optional, collection = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
               name: "objectWithId",
               routeTemplate: "1/objects/{name}/{id}",
               defaults: new { controller = "viewData", name = RouteParameter.Optional, id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "viewData",
                routeTemplate: "1/view/data/{name}",
                defaults: new { controller = "viewData", name = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "cloudServiceProvider",
                routeTemplate: "admin/function/provider",
                defaults: new { controller = "viewData", name = "cloudServiceProvider" }
            );

            config.Routes.MapHttpRoute(
                name: "tableData",
                routeTemplate: "1/table/data/{name}",
                defaults: new { controller = "viewData", name = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "object",
                routeTemplate: "1/objects/{name}",
                defaults: new { controller = "viewData", name = RouteParameter.Optional }
            );

            

            config.Routes.MapHttpRoute(
                name: "appConfig",
                routeTemplate: "1/app/config",
                defaults: new { controller = "appConfig" }
            );

            config.Routes.MapHttpRoute(
                name: "viewConfig",
                routeTemplate: "1/view/config/{name}",
                defaults: new { controller = "viewConfig" }
            );

            config.Routes.MapHttpRoute(
                name: "businessRule",
                routeTemplate: "1/businessRule/{id}",
                defaults: new { controller = "businessRule" }
            );

            config.Routes.MapHttpRoute(
                name: "action",
                routeTemplate: "1/action/config/{id}",
                defaults: new { controller = "businessRule" }
            );

            //config.Routes.MapHttpRoute(
            //    name: "performAction",
            //    routeTemplate: "1/table/action/{viewName}/{id}",
            //    defaults: new { controller = "rule", action = "Perform" }
            //);

            config.Routes.MapHttpRoute(
                name: "tableConfig",
                routeTemplate: "1/table/config/{name}",
                defaults: new { controller = "viewConfig" }
            );

            config.Routes.MapHttpRoute(
               name: "ConfigSchema",
               routeTemplate: "1/table/config/template",
               defaults: new { controller = "viewConfig" }
           );

            config.Routes.MapHttpRoute(
                name: "fileUpload",
                routeTemplate: "1/file/upload/{viewName}/{fieldName}",
                defaults: new { controller = "file", action = "Upload" }
            );

            config.Routes.MapHttpRoute(
                name: "file2Upload",
                routeTemplate: "1/file2/",
                defaults: new { controller = "file2" }
            );


            config.Routes.MapHttpRoute(
                name: "viewListConfig",
                routeTemplate: "1/view/config",
                defaults: new { controller = "viewConfig" }
            );

            config.Routes.MapHttpRoute(
                name: "actionList",
                routeTemplate: "1/action/config",
                defaults: new { controller = "businessRule" }
            );
            config.Routes.MapHttpRoute(
                name: "businessRuleList",
                routeTemplate: "1/businessRule",
                defaults: new { controller = "businessRule" }
            );

            config.Routes.MapHttpRoute(
                name: "tableListConfig",
                routeTemplate: "1/table/config",
                defaults: new { controller = "viewConfig" }
            );

            //config.Routes.MapHttpRoute(
            //    name: "banner",
            //    routeTemplate: "1/banner/getAdminUrl",
            //    defaults: new { controller = "banner", action = "getAdminUrl" }
            //);

            config.Routes.MapHttpRoute(
                name: "autoComplete",
                routeTemplate: "1/view/data/autocomplete/{viewName}/{fieldName}",
                defaults: new { controller = "viewData", action = "autoComplete" }
            );

            config.Routes.MapHttpRoute(
                name: "selectOptions",
                routeTemplate: "1/view/data/selectOptions/{viewName}/{fieldName}",
                defaults: new { controller = "viewData", action = "selectOptions" }
            );

            config.Routes.MapHttpRoute(
                name: "autoComplete2",
                routeTemplate: "1/table/data/autocomplete/{viewName}/{fieldName}",
                defaults: new { controller = "viewData", action = "autoComplete" }
            );

            config.Routes.MapHttpRoute(
                name: "selectOptions2",
                routeTemplate: "1/table/data/selectOptions/{viewName}/{fieldName}",
                defaults: new { controller = "viewData", action = "selectOptions" }
            );

            config.Routes.MapHttpRoute(
                name: "autoComplete3",
                routeTemplate: "1/objects/autocomplete/{viewName}/{fieldName}",
                defaults: new { controller = "viewData", action = "autoComplete" }
            );

            config.Routes.MapHttpRoute(
                name: "selectOptions3",
                routeTemplate: "1/objects/selectOptions/{viewName}/{fieldName}",
                defaults: new { controller = "viewData", action = "selectOptions" }
            );

            //config.Routes.MapHttpRoute(
            //    name: "sync",
            //    routeTemplate: "1/app/sync",
            //    defaults: new { controller = "appConfig", action = "sync" }
            //);
            config.Routes.MapHttpRoute(
                name: "refresh",
                routeTemplate: "1/config/refresh/{appname}",
                defaults: new { controller = "appConfig", action = "refresh" }
            );

            config.Routes.MapHttpRoute(
                name: "unlock",
                routeTemplate: "1/account/unlock",
                defaults: new { controller = "account", action = "unlock" }
            );

            //admin
            config.Routes.MapHttpRoute(
               name: "myAppsGet",
               routeTemplate: "admin/myApps/{id}",
               defaults: new { controller = "myApps", id = RouteParameter.Optional }
           );

          //  config.Routes.MapHttpRoute(
          //    name: "myAppsStatus",
          //    routeTemplate: "admin/myAppsStatus/{id}",
          //    defaults: new { controller = "myApps", action = "status", id = RouteParameter.Optional }
          //);

            config.Routes.MapHttpRoute(
               name: "myAppConnection",
               routeTemplate: "admin/myAppConnection/{id}",
               defaults: new { controller = "myAppConnection", id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
              name: "myAppConnectionPassword",
              routeTemplate: "admin/myAppConnection/getPassword/{id}",
              defaults: new { controller = "myAppConnection", action = "getPassword", id = RouteParameter.Optional }
           );
			
            config.Routes.MapHttpRoute(
              name: "myAppConnectionTest",
              routeTemplate: "admin/myAppConnectionTest",
              defaults: new { controller = "myAppConnection", action = "Test" }
          );

            config.Routes.MapHttpRoute(
              name: "user",
              routeTemplate: "1/user",
              defaults: new { controller = "user" }
          );

           
           // config.Routes.MapHttpRoute(
           //    name: "myAppsPost",
           //    routeTemplate: "admin/myApps",
           //    defaults: new { controller = "myApps", action = "Post" }
           //);
           // config.Routes.MapHttpRoute(
           //    name: "myAppsPut",
           //    routeTemplate: "admin/myApps/{id}",
           //    defaults: new { controller = "myApps", action = "Put", id = RouteParameter.Optional }
           //);

            //config.Routes.MapHttpRoute(
            //    name: "myApps",
            //    routeTemplate: "admin/myApps",
            //    defaults: new { controller = "myApps", action = "Get" }
            //);


            config.Routes.MapHttpRoute(
                           name: "ExternalLogin",
                           routeTemplate: "account/ExternalLogin", defaults: new { });
                

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            /*var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            */

            config.Formatters.Clear();

            config.Formatters.Add(new TextPlainFormatter());
            config.MessageHandlers.Add(new MethodOverrideHandler());
            config.Formatters.Add(new JsonMediaTypeFormatter());
            
            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new JsonMediaTypeFormatter());

            config.Services.Replace(typeof(IHostBufferPolicySelector), new NoBufferPolicySelector());

 
        }
    }

    public class NoBufferPolicySelector : WebHostBufferPolicySelector
    {
        public override bool UseBufferedInputStream(object hostContext)
        {
            var context = hostContext as HttpContextBase;

            string url = null;
            try
            {
                url = (((Microsoft.Owin.OwinContext)(hostContext)).Request).Uri.AbsolutePath;
            }
            catch { }

            if (url != null)
            {
                if (url.Contains("1/file"))
                    return false;

                string method = (((Microsoft.Owin.OwinContext)(hostContext)).Request).Method;
                if (url.Contains("/action/") && method.ToUpper().Equals("POST"))
                    return false;
            }

            return true;
        }

        public override bool UseBufferedOutputStream(HttpResponseMessage response)
        {
            return base.UseBufferedOutputStream(response);
        }
    }

    public class MethodOverrideHandler : DelegatingHandler
    {
        readonly string[] _methods = { "DELETE", "HEAD", "PUT", "GET" };
        const string _header = "X-HTTP-Method-Override";

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Check for HTTP POST with the X-HTTP-Method-Override header.
            if (request.Method == HttpMethod.Post && request.Headers.Contains(_header))
            {
                // Check if the header value is in our methods list.
                var method = request.Headers.GetValues(_header).FirstOrDefault();
                if (_methods.Contains(method, StringComparer.InvariantCultureIgnoreCase))
                {
                    // Change the request method.
                    request.Method = new HttpMethod(method);
                }
            }
            return base.SendAsync(request, cancellationToken);
        }
    }

    public class TextPlainFormatter : MediaTypeFormatter
    {
        public TextPlainFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, System.Net.Http.HttpContent content, IFormatterLogger formatterLogger)
        {
            return Task.Factory.StartNew(() =>
            {
                StreamReader reader = new StreamReader(readStream);
                return (object)reader.ReadToEnd();
            });
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                StreamWriter writer = new StreamWriter(writeStream);
                writer.Write(value);
                writer.Flush();
            });
        }

        
    }
}
