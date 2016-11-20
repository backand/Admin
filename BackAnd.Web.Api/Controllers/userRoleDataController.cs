using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using System.Net;
using System.Net.Http;
using Durados.Web.Mvc;
using Durados.Web.Mvc.UI.Helpers;
using BackAnd.Web.Api.Controllers.Filters;

namespace BackAnd.Web.Api.Controllers
{
    [RoutePrefix("1/table/data/durados_UserRole")]
    [BackAndAuthorize("Admin")]
    public class userRoleDataController : viewDataController
    {
        string roleTableName = "durados_UserRole";
        [Route("{id}")]
        public virtual IHttpActionResult Get(string id, bool? deep = null)
        {
            return base.Get(roleTableName, id, deep);
        }
        [Route("")]
        public virtual IHttpActionResult Get(bool? withSelectOptions = null, bool? withFilterOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null, bool? deep = null, bool descriptive = true)
        {
            return base.Get(roleTableName, withSelectOptions, withFilterOptions, pageNumber, pageSize, filter, sort, search, deep, descriptive);
        }
        [Route("{id}")]
        public IHttpActionResult Put(string id, bool? deep = null, bool? returnObject = null, string parameters = null)
        {
            var validateMessage = ValidateRestrictions(id);
            if (validateMessage != null)
                return validateMessage;
            return base.Put(roleTableName, id, deep, returnObject, parameters);
        }

        [Route("")]
        public virtual IHttpActionResult Post(bool? deep = null, bool? returnObject = null, string parameters = null)
        {

            return base.Post(roleTableName, deep, returnObject, parameters);
        }

        [Route("{id}")]
        public virtual IHttpActionResult Delete(string id, bool? deep = null, string parameters = null)
        {
            var validateMessage = ValidateRestrictions(id);
            if (validateMessage != null)
                return validateMessage;
            return base.Delete(roleTableName, id, deep, parameters);
        }

        private IHttpActionResult ValidateRestrictions(string id)
        {
            if (string.IsNullOrEmpty(id))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Role name can not be empty"));
            if (id == "Admin")
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Can not add/edit/delete Admin role name"));
            if (id == "Developer")
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Can not add/edit/delete Developer role name"));
            if (id == "User")
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Can not add/edit/delete User role name"));
            return null;
        }

    }

    [RoutePrefix("1/table/data/v_durados_User")]
    [BackAndAuthorizeAttribute("Admin")]
    public class userDataController : viewDataController
    {
        string userTableName = "v_durados_User";
        [Route("{id}")]
        [HttpGet]
        public virtual IHttpActionResult Get(string id, bool? deep = null)
        {
            return base.Get(userTableName, id, deep);
        }
        [Route("")]
        public virtual IHttpActionResult Get(bool? withSelectOptions = null, bool? withFilterOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null, bool? deep = null, bool descriptive = true)
        {
            return base.Get(userTableName, withSelectOptions, withFilterOptions, pageNumber, pageSize, filter, sort, search, deep, descriptive);
        }

        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult Put(string id, bool? deep = null, bool? returnObject = null, string parameters = null)
        {


            string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

            if (string.IsNullOrEmpty(json))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.MissingObjectToUpdate));
            }
            View view = GetView(userTableName);
            if (view == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, userTableName)));
            }
            Dictionary<string, object> values = ((Durados.Web.Mvc.View)view).Deserialize(json);
            if (values.ContainsKey("durados_User_Role") && values["durados_User_Role"].ToString().ToLower() == "developer")
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "can not assign  developer role"));
            }
            if (values.ContainsKey("Username") && view.Database.GetUsernameById(id) != values["Username"].ToString())
            {

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "can not change username"));
            }
            return base.Put(userTableName, id, deep, returnObject, parameters);
        }
        [Route("{id}")]
        public virtual IHttpActionResult Delete(string id, bool? deep = null, string parameters = null)
        {
            return base.Delete(userTableName, id, deep, parameters);
        }
        [Route("")]
        public virtual IHttpActionResult Post(bool? deep = null, bool? returnObject = null, string parameters = null)
        {
            return base.Post(userTableName, deep, returnObject, parameters);
        }


    }
}