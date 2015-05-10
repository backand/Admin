using System;
//﻿using Backand.Web.Api;
using BackAnd.Web.Api.Models;
//using BackAnd.Web.Api.Providers;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Durados.Web.Mvc.Controllers;
using System.Web;
using Microsoft.Owin.Security.Infrastructure;
using Owin.Security.Providers.GitHub;
using Backand.Web.Api;
using BackAnd.Web.Api.Providers;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc;


namespace BackAnd.Web.Api.Controllers
{


    [RoutePrefix("1/user")]
    public class userController : wfController
    {
        [Route("signup")]
        [HttpPost]
        [BackAnd.Web.Api.Controllers.Filters.TokenAuthorize(BackAnd.Web.Api.Controllers.Filters.HeaderToken.SignUpToken)]
        public virtual IHttpActionResult SignUp()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);
                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (!values.ContainsKey("firstName"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "First name is missing"));

                }

                string firstName = values["firstName"].ToString();


                if (!values.ContainsKey("email"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Email is missing"));
                }

                string email = values["email"].ToString();

                if (!values.ContainsKey("lastName"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Last name is missing"));

                }
                string lastName = values["lastName"].ToString();
                

                if (!values.ContainsKey("password"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Password is missing"));

                }
                string password = values["password"].ToString();

                if (!values.ContainsKey("confirmPassword"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Confirm password is missing"));

                }
                string confirmPassword = values["confirmPassword"].ToString();

                if (!password.Equals(confirmPassword))
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The password is is not confirmed"));


                Dictionary<string, object> parameters = null;
                if (values.ContainsKey("parameters"))
                {
                    if (!(values["parameters"] is Dictionary<string, object>))
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Could not convert parameters to object"));
                    }
                    parameters = (Dictionary<string, object>)values["parameters"];
                }

                if (!System.Web.HttpContext.Current.Items.Contains(Durados.Web.Mvc.Database.AppName))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "App not found"));
                }
                string appName = System.Web.HttpContext.Current.Items[Durados.Web.Mvc.Database.AppName].ToString();
                Account account = new Account(this);

                try
                {
                    Durados.Web.Mvc.UI.Helpers.Account.SignUpResults signUpResults = account.SignUp(appName, firstName, lastName, email, password, confirmPassword, parameters, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                    var response = new { username = signUpResults.Username, currentStatus = (int)signUpResults.Status, message = account.GetSignUpStatusMessage(signUpResults.Status), listOfPossibleStatus = account.GetListOfPossibleStatus() };
                    return Ok(response);
                }
                catch (Durados.Web.Mvc.UI.Helpers.Account.SignUpException exception)
                {
                    Log(appName, exception, 3);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, exception.Message));
                }
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private Map GetMap(string appName)
        {
            Map map = Maps.Instance.GetMap(appName);

            return map;
        }

        protected virtual void Log(string appName, Exception exception, int logType)
        {
            GetMap(appName).Logger.Log(GetControllerNameForLog(ControllerContext), string.Empty, string.Empty, exception, logType, null);

        }

        protected virtual void Log(Exception exception, int logType)
        {
            Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), string.Empty, string.Empty, exception, logType, null);

        }

        [Route("verify")]
        [HttpGet]
        public virtual HttpResponseMessage Verify(string appName, string parameters)
        {
            if (string.IsNullOrEmpty(appName))
                return Request.CreateResponse(HttpStatusCode.NotFound, "appName cannot be empty");
            Map map = GetMap(appName);
            if (map == null || map is DuradosMap)
                return Request.CreateResponse(HttpStatusCode.NotFound, "app was not found");

            if (string.IsNullOrEmpty( map.Database.SignInRedirectUrl))
                return Request.CreateResponse(HttpStatusCode.NotFound, "SignIn redirect url was not supplied in configuration");
            if (string.IsNullOrEmpty( map.Database.RegistrationRedirectUrl))
                return Request.CreateResponse(HttpStatusCode.NotFound, "SignUp redirect url was not supplied in configuration");

            try
            {
                Account account = new Account(this);

                try
                {
                    Durados.Web.Mvc.UI.Helpers.Account.SignUpResults signUpResults = account.Verify(appName, parameters, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
                    var response = Request.CreateResponse(HttpStatusCode.Moved);
                    response.Headers.Location = new Uri(signUpResults.Redirect);
                    return response;
                }
                catch (Durados.Web.Mvc.UI.Helpers.Account.SignUpException exception)
                {
                    Log(exception, 3);
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, exception.Message);
                }
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

    }
}