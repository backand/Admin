using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Controllers
{
    public class MultiTenancyController : CrmController
    {
        string nameFieldName = "Name";
        string dataSourceTypeFieldName = "FK_durados_App_durados_DataSourceType_Parent";

        protected override UI.TableViewer GetNewTableViewer()
        {
            return new UI.MultiTenancyTableViewer();
        }
        private string GetCleanName(string name)
        {
            name = name.Trim();

            if (name.ToLower().Equals("www"))
                throw new DuradosException(Map.Database.Localizer.Translate("This name already exists."));

            Regex regex = new Regex("^[A-Za-z0-9]+$");
            if (!regex.IsMatch(name))
            {
                throw new DuradosException(Map.Database.Localizer.Translate("The 'Name' is the first part of the URL of the App.\n\rOnly alphanumeric characters are allowed."));
            }

            if (name.Length > Maps.AppNameMax)
                throw new DuradosException(Map.Database.Localizer.Translate("The 'Name' is the first part of the URL of the App.\n\rMaximum 25 characters are allowed."));
            
            
            return name;
        }

        protected override void BeforeCreate(CreateEventArgs e)
        {
            string urlFieldName = "Url";
            
            string name = e.Values[nameFieldName].ToString();

            string dataSourceTypeId = e.Values[dataSourceTypeFieldName].ToString();


            SqlAccess sqlAccess = new SqlAccess();
            Dictionary<string, object> values = new Dictionary<string, object>();

            string cleanName = GetCleanName(name);

            values.Add("newdb", cleanName);
            values.Add("newSysDb", Maps.DuradosAppSysPrefix + cleanName);
            if (dataSourceTypeId == "1" || dataSourceTypeId == "4") // blank or template
            {
                try
                {
                    sqlAccess.ExecuteNoneQueryStoredProcedure(Maps.Instance.ConnectionString, "durados_CreateNewDatabase", values);
                }
                catch (SqlException exception)
                {
                    throw new DuradosException(exception.Message, exception);
                }
            }
            else if (dataSourceTypeId == "4") // template
            {
                //sqlAccess.ExecuteNoneQueryStoredProcedure(Maps.Instance.ConnectionString, "durados_CreateNewDatabase", values);
            }


            if (!e.Values.ContainsKey(urlFieldName))
            {
                e.Values.Add(urlFieldName, string.Empty);
            }

            string port = Request.Url.Port.ToString();
            string host = Maps.Host;

            if (Request.Url.ToString().Contains(port))
                host += ":" + port;

            e.Values[urlFieldName] = string.Format("{0}.{1}|_blank|" + System.Web.HttpContext.Current.Request.Url.Scheme + "://{0}.{1}?appName={0}", cleanName, host);

            string uploadPath = GetUploadPath((ColumnField)e.View.Fields["Image"]);
            string image = e.Values["Image"].ToString();
            string path = uploadPath + image;
            string newPath = uploadPath + cleanName + "\\" + image;
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(newPath);
            fileInfo.Directory.Create(); // If the directory already exists, this method does nothing.

            try
            {
                System.IO.File.Copy(path, newPath);
            }
            catch { }

            base.BeforeCreate(e);
        }

        protected override void AfterCreateAfterCommit(CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);

            string dataSourceTypeId = e.Values[dataSourceTypeFieldName].ToString();
            string name = e.Values[nameFieldName].ToString();
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = Map.connectionString;

            string cleanName = GetCleanName(name);

            if (dataSourceTypeId == "1" || dataSourceTypeId == "4") // blank or template
            {
                SqlAccess sqlAccess = new SqlAccess();
                
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("Catalog", cleanName);
                values.Add("SysCatalog", Maps.DuradosAppSysPrefix + cleanName);
                //values.Add("ServerName", builder.DataSource);
                //values.Add("Username", builder.UserID);
                //values.Add("Password", builder.Password);
                //values.Add("IntegratedSecurity", builder.IntegratedSecurity);
                values.Add("DuradosUser", Map.Database.GetUserID());
                sqlAccess.ExecuteNoneQueryStoredProcedure(Maps.Instance.ConnectionString, "durados_SetConnection", values);

           }
            if (dataSourceTypeId == "2") // existing
            {
                string sysConnection = e.Values["FK_durados_App_durados_SqlConnection_System_Parent"].ToString();
                if (string.IsNullOrEmpty(sysConnection))
                {
                    SqlAccess sqlAccess = new SqlAccess();

                    Dictionary<string, object> values = new Dictionary<string, object>();
                    values.Add("Catalog", cleanName);
                    values.Add("SysCatalog", Maps.DuradosAppSysPrefix + cleanName);
                    values.Add("DuradosUser", Map.Database.GetUserID());
                    sqlAccess.ExecuteNoneQueryStoredProcedure(Maps.Instance.ConnectionString, "durados_SetSysConnection", values);
                }
            }

            if (dataSourceTypeId == "4") // template
            {
                IPersistency persistency = Maps.Instance.GetNewPersistency();
                MapDataSet.durados_AppRow appRow = (MapDataSet.durados_AppRow)((View)e.View).GetDataRow(e.View.Fields[nameFieldName], name);

                if (!appRow.IsTemplateFileNull() && !string.IsNullOrEmpty(appRow.TemplateFile))
                {
                    TemplateGenerator TemplateGenerator = new TemplateGenerator(persistency.GetConnection(appRow, builder).ToString(), appRow.TemplateFile);
                }
            }

            //UpdateCache(name);
            CreateDns(cleanName);
        }

        //protected virtual void UpdateCache(string name)
        //{
        //    Maps.Instance.AddMap(name);
        //}

        protected virtual void CreateDns(string name)
        {
            if (Maps.Debug)
            {
                string windowsPath = System.Environment.GetEnvironmentVariable("windir");

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(windowsPath + @"\system32\drivers\etc\hosts", true))
                {

                    sw.WriteLine(string.Format("127.0.0.1   {0}", name + "." + Maps.Host));

                    sw.Close();

                }
            }
        }

        public class TemplateGenerator : Durados.DataAccess.AutoGeneration.Generator
        {
            public TemplateGenerator(string connectionString, string schemaGeneratorFileName)
                : base(connectionString, schemaGeneratorFileName)
            {
            }

            protected override string RootObjectName
            {
                get { return string.Empty; }
            }

            protected override bool SchemaExists(string connectionString)
            {
                return false;
            }
        }

    }

}



