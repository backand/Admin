using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Durados.Web.Mvc;
using Durados.Web.Mvc.UI.Helpers;
using System.Collections;
using Newtonsoft.Json;

namespace BackAnd.AspNetCore.Api.Controllers
{
    [Route("api/[controller]")]
    public class ApiController : Controller
    {
        protected Map map = null;
        public Map Map
        {
            get
            {
                if (map == null)
                    map = Maps.Instance.GetMap();
                map.OpenSshSession();
                return map;
            }
        }

        protected internal virtual Durados.Web.Mvc.View GetView(string viewName)
        {
            Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Map.Database.GetViewByJsonName(viewName);
            if (view != null)
                return view;

            return ViewHelper.GetViewForRest(viewName);
        }

        protected virtual bool IsAllow(View view)
        {
            if (view.SystemView && view.Database.Map.IsMainMap)
            {
                return false;
            }
            if (Map.Database.GetCurrentUsername() == Durados.Database.GuestUsername && (view.Database.IsConfig || view.SystemView))
                return false;
            return view.IsAllow();
        }

        protected void ReplaceVariables(Dictionary<string, object> result)
        {
            if (result.ContainsKey("values") && result.ContainsKey("variables") && result.ContainsKey("str") && result.ContainsKey("where"))
            {
                foreach (string variable in (ArrayList)result["variables"])
                {
                    string value = ((Dictionary<string, object>)result["values"])[variable].ToString();
                    if (!(value.StartsWith("'") && value.EndsWith("'")))
                    {
                        decimal d = 0;
                        if (!decimal.TryParse(value, out d))
                        {
                            value = value.Pad("'");
                        }
                    }
                    result["str"] = result["str"].ToString().Replace(variable, value);
                    result["where"] = result["where"].ToString().Replace(variable, value);
                }
            }
        }

        protected virtual Dictionary<string, object> transformJson(string json, bool shouldGeneralize = false)
        {
            string getNodeUrl = GetNodeUrl() + "/transformJson";

            bulk bulk = new Durados.Web.Mvc.UI.Helpers.bulk();

            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            if (!data.ContainsKey("appName"))
            {
                string appName = Map.AppName;
                if (string.IsNullOrEmpty(appName))
                {
                    appName = (System.Web.HttpContext.Current.Items[Durados.Web.Mvc.Database.AppName] ?? string.Empty).ToString();
                }
                data.Add("appName", appName);
            }
            if (shouldGeneralize)
            {
                data.Add("shouldGeneralize", shouldGeneralize);
            }

            json = jss.Serialize(data);

            var tasks = new List<Task<string>>();
            object responses = null;
            tasks.Add(Task.Factory.StartNew(() =>
            {
                Dictionary<string, object> headers = new Dictionary<string, object>() { { "Content-Type", "application/json" } };

                // in transformJson take app creator credential to allow get BakandToObject
                headers.Add("Authorization", Map.GetAuthorization());
                var responseStatusAndData = bulk.GetWebResponse("POST", getNodeUrl, json, null, headers, 0);
                responses = responseStatusAndData.data;
                if (string.IsNullOrEmpty(responseStatusAndData.data))
                {
                    if (responseStatusAndData.GetHeaders()["error"] != null && !string.IsNullOrEmpty(responseStatusAndData.GetHeaders()["error"].ToString()))
                    {
                        throw new DuradosException(responseStatusAndData.GetHeaders()["error"].ToString());
                    }
                }
                return responseStatusAndData.data;
            }));

            Task.WaitAll(tasks.ToArray());

            Dictionary<string, object> result = null;
            try
            {
                result = jss.Deserialize<Dictionary<string, object>>(responses.ToString());
            }
            catch
            {
                throw new DuradosException(responses.ToString());
            }

            if (result == null)
            {
                LogModel(json, string.Empty, string.Empty, "nosql");
            }
            else
            {
                LogModel(json, new JavaScriptSerializer().Serialize(result), result.ContainsKey("valid") ? result["valid"].ToString() : string.Empty, "nosql");
            }

            return result;



        }

    }

    public class Messages
    {
        public static readonly string FieldShouldBeParent = "The field \"{0}\" nust be a single-select or a multi-select type.";
        public static readonly string ForeignKeyDeleteViolation = "The row you are trying to delete is referenced in another table ,please check your table constraints";
        public static readonly string FieldShouldBeAutoComplete = "The field \"{0}\" nust be an autocomplete display format.";
        public static readonly string ViewNameIsMissing = "The object name is missing.";
        public static readonly string FieldNameIsMissing = "The field name is missing.";
        public static readonly string IdIsMissing = "The id is missing.";
        public static readonly string CollectionIsMissing = "The collection is missing.";
        public static readonly string CollectionNotFound = "The collection was not found.";
        public static readonly string DuplicateCollectionName = "The collection name exists more than once.";
        public static readonly string ViewIsUnauthorized = "The object is unauthorized for this current user role.";
        public static readonly string ViewNameNotFound = "The object \"{0}\" was not found.";
        public static readonly string PostContradictsPredefinedFilter = "Post failed because it contradicts the predefined filter.";

        public static readonly string MissingObjectToUpdate = "The object to update is missing.";
        public static readonly string FieldNameNotFound = "The field \"{0}\" was not found.";
        public static readonly string TheFieldMustBeTextual = "The field must be textual.";
        public static readonly string ItemWithIdNotFound = "An item with id \"{0}\" was either not found, not allowed to the current role or filtered out by a predefined filter in the object \"{1}\".";
        public static readonly string ItemWithNoFieldsToUpdate = "An item with id \"{0}\" has no fields to update in the object \"{1}\".";
        public static readonly string AppNotFound = "The app \"{0}\" was not found.";
        public static readonly string ChartWithIdNotFound = "An chart with id \"{0}\" was not found.";
        public static readonly string Unexpected = "An error occurred, please try again or contact the administrator. Error details: {0}";
        public static readonly string Critical = "Critical Exception";
        public static readonly string ActionIsUnauthorized = "The action is unauthorized for this current user role.";
        public static readonly string WorkspaceNotFound = "The workspace was not found";
        public static readonly string WorkspaceNameMissing = "The workspace name is missing";
        public static readonly string WorkspaceWithNameAlreadyExists = "A Workspace with the name {0} already exists.";
        public static readonly string QueryWithNameAlreadyExists = "A Query with the name {0} already exists.";
        public static readonly string CronWithNameAlreadyExists = "A Cron with the name {0} already exists.";
        public static readonly string UploadWithNameAlreadyExists = "An Upload with the name {0} already exists.";
        public static readonly string ChangeAdminWorkspaceNameNotAllowed = "Changing Admin worksapces name is not allowed.";
        public static readonly string WorkspaceLimit = "You have reached workspaces limit";
        public static readonly string UploadNotFound = "The column has no upload configuration";
        public static readonly string InvalidFileType = "Invalid file type";
        public static readonly string InvalidFileType2 = "Invalid file type in field [{0}].<br><br>Allowed formats: {1}";
        public static readonly string AppNameAlreadyExists = "An application by the name {0} already exists.";
        public static readonly string AppNameCannotBeNull = "App name cannot be empty.";
        public static readonly string AppNameInvalid = "App name must be alphanumeric.";
        public static readonly string ActionWithNameAlreadyExists = "An action with the name {0} already exists for table {1}.";
        public static readonly string FunctionWithNameAlreadyExists = "A function with the name {0} already exists.";
        public static readonly string RuleNotFound = "The action does not exist.";
        public static readonly string NotImplemented = "The action is not implemented yet.";
        public static readonly string FailedToGetJsonFromParameters = "Failed to get json from parameters.";
        public static readonly string StringifyFilter = "Please JSON.stringify the filter parameter";
        public static readonly string StringifyBulk = "Failed to parse the Bulk JSON";
        public static readonly string GetFilterError = "Failed to translate filter";
        public static readonly string StringifySort = "Please JSON.stringify the sort parameter";
        public static readonly string StringifyFields = "Please provide the fields parameters as ['field1','field2']";
        public static readonly string InvalidSchema = "Invalid schema";

        public static readonly string MoreThanOneParseConversions = "{0} has more than one parse conversions.";
        public static readonly string MigrationAlreadyStartedWithoutGettingItsStatus = "{0} has already created a migration request.";
        public static readonly string MigrationAlreadyStartedWithStatusIdle = "{0} has already created a migration request and it is now waiting to start.";
        public static readonly string MigrationAlreadyStartedWithStatusStarted = "{0} has already started its migration.";
        public static readonly string MigrationAlreadyStartedWithStatusFinished = "{0} has already finished its migration. If you want to migrate again please create a new app.";
        public static readonly string NotSignInToApp = "Please sign in to an app";
        public static readonly string CloudVendorNotFound = "This cloud vendor is not currently supported";
        public static readonly string CloudTypeNotFound = "This cloud service type is not currently supported";
        public static readonly string DuplicateCloudName = "This account name is already in use";
        public static readonly string MissingCloudName = "Please provide the account name";

    }
}
