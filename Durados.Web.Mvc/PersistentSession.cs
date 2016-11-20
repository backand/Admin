using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc
{
    [Serializable()]
    public class PersistentSession : Durados.DataAccess.AutoGeneration.PersistentSession
    {
        public PersistentSession(string connectionString, string sessionSchemaGeneratorFileName)
            : base(connectionString, sessionSchemaGeneratorFileName)
        {
        }

        public object this[string name]
        {
            get
            {
                try
                {
                    if (System.Web.HttpContext.Current == null)
                        return null;
                    if (Maps.Instance.IsApi())
                    {
                        return System.Web.HttpContext.Current.Items[name];
                    }
                    
                    if (System.Web.HttpContext.Current.Session == null)
                    {
                        if (System.Web.HttpContext.Current.Items[name] == null && System.Web.HttpContext.Current.Items[Database.RequestId] != null)
                        {
                            System.Web.HttpContext.Current.Items[name] = base.GetSession(name, System.Web.HttpContext.Current.Items[Database.RequestId].ToString());
                        }
                        return System.Web.HttpContext.Current.Items[name];
                    }
                    else if (System.Web.HttpContext.Current.Session[name] == null)
                    {
                        System.Web.HttpContext.Current.Session[name] = base.GetSession(name, System.Web.HttpContext.Current.Session.SessionID);
                    }
                    return System.Web.HttpContext.Current.Session[name];
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    if (Maps.Instance.IsApi())
                    {
                        System.Web.HttpContext.Current.Items[name] = value;
                        return;
                    }
                    if (System.Web.HttpContext.Current.Session == null)
                    {
                        System.Web.HttpContext.Current.Items[name] = value;
                        if (System.Web.HttpContext.Current.Items[Database.RequestId] != null)
                        {
                            base.SetSession(name, System.Web.HttpContext.Current.Items[Database.RequestId].ToString(), value);
                        }
                    }
                    else
                    {
                        System.Web.HttpContext.Current.Session[name] = value;
                        base.SetSession(name, System.Web.HttpContext.Current.Session.SessionID, value);
                    }
                }
                catch { }
            }
        }

    }
    public class MySqlPersistentSession:PersistentSession
    {
        public MySqlPersistentSession(string connectionString, string sessionSchemaGeneratorFileName)
            : base(connectionString, sessionSchemaGeneratorFileName)
        {
        }
        protected override SqlSchema GetNewSqlSchema()
        {
            return new Durados.DataAccess.MySqlSchema();
        }
        protected override string GetSessionSelectStatement()
        {
            return "select Scalar, TypeCode, SerializedObject, ObjectType from durados_session where SessionID=@SessionID and Name=@Name";
        }
    }
}
