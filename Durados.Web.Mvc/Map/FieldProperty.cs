using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Infrastructure;
using Durados.Web.Mvc.Azure;
using Newtonsoft.Json;
using System.Diagnostics;
using Durados.Data;
using Durados.SmartRun;
using Durados.Web.Mvc.Farm;

namespace Durados.Web.Mvc
{
    public class FieldProperty
    {
        private Type columnType = typeof(Durados.Web.Mvc.ColumnField);
        private Type parentType = typeof(Durados.Web.Mvc.ParentField);
        private Type childrenType = typeof(Durados.Web.Mvc.ChildrenField);

        //private Dictionary<string, Dictionary<string, bool>> properties = new Dictionary<string, Dictionary<string, bool>>();
        private Durados.Data.ICache<Durados.Data.ICache<bool>> properties = CacheFactory.CreateCache<Durados.Data.ICache<bool>>("properties");

        public bool IsInType(string fieldName, string fieldType)
        {
            Type type = GetFieldType(fieldType);

            return Has(type, fieldType, fieldName);
        }


        private bool Has(Type type, string fieldType, string fieldName)
        {
            if (!properties.ContainsKey(fieldType))
                properties.Add(fieldType, CacheFactory.CreateCache<bool>(fieldType));

            if (!properties[fieldType].ContainsKey(fieldName))
                properties[fieldType].Add(fieldName, type != null && type.GetProperty(fieldName) != null);

            return properties[fieldType][fieldName];
        }

        private Type GetFieldType(string fieldType)
        {
            if (fieldType == FieldType.Children.ToString())
                return childrenType;
            else if (fieldType == FieldType.Column.ToString())
                return columnType;
            else if (fieldType == FieldType.Parent.ToString())
                return parentType;
            else
                return null;
        }
    }
}
