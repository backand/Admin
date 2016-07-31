using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.IO;
using System.Xml;

namespace Durados
{
    public class Configuration
    {
        public virtual void Load(Database db, string configFileName)
        {
            if (string.IsNullOrEmpty(configFileName))
            {
                throw new ConfigFileNamePropertyMissingException();
            }

            string fileName = configFileName.StartsWith("~") ? HttpContext.Current.Server.MapPath(configFileName) : configFileName;

            if (!File.Exists(fileName))
            {
                throw new ConfigFileDoesNotExistException(fileName);
            }

            try
            {
                using (XmlTextReader textReader = new XmlTextReader(fileName))
                {
                    textReader.Read();
                    // If the node has value
                    View view = null;
                    Field field = null;
                    object parent = null;
                    while (textReader.Read())
                    {
                        textReader.MoveToElement();
                        if (textReader.Name == "Database")
                        {
                            PropertyInfo[] properties = db.GetType().GetProperties();

                            LoadConfigurableProperties(properties, db, textReader);

                            parent = db;


                        }
                        else if (textReader.Name == "View")
                        {
                            string viewName = textReader.GetAttribute("Name");
                            if (!string.IsNullOrEmpty(viewName) && db.Views.ContainsKey(viewName))
                            {
                                view = db.Views[viewName];
                                parent = view;

                                PropertyInfo[] properties = view.GetType().GetProperties();

                                LoadConfigurableProperties(properties, view, textReader);

                            }



                        }
                        else if (textReader.Name == "Field")
                        {
                            string fieldName = textReader.GetAttribute("Name");
                            if (!string.IsNullOrEmpty(fieldName) && view != null && view.Fields.ContainsKey(fieldName))
                            {
                                field = view.Fields[fieldName];
                                parent = field;

                                PropertyInfo[] properties = field.GetType().GetProperties();

                                LoadConfigurableProperties(properties, field, textReader);

                            }
                        }
                        else
                        {
                            string typeName = textReader.Name;
                            if (parent != null && !string.IsNullOrEmpty(typeName))
                            {
                                PropertyInfo propertyInfo = parent.GetType().GetProperty(typeName);
                                if (propertyInfo != null)
                                {
                                    object propertyValue = propertyInfo.GetValue(parent, null);
                                    if (propertyValue == null)
                                    {
                                        propertyValue = System.Activator.CreateInstance(propertyInfo.PropertyType);
                                        propertyInfo.SetValue(parent, propertyValue, null);
                                    }
                                    parent = propertyValue;
                                    PropertyInfo[] properties = propertyValue.GetType().GetProperties();

                                    LoadConfigurableProperties(properties, propertyValue, textReader);
                                }
                            }
                        }

                        
                    }
                }
            }
            catch (Exception exception)
            {
                throw new ReadingConfigFileException(exception);
            }
        }

        protected virtual void LoadConfigurableProperties(PropertyInfo[] properties, object o, XmlTextReader textReader)
        {
            foreach (PropertyInfo propertyInfo in properties)
            {
                object[] configurableAttributes = propertyInfo.GetCustomAttributes(typeof(ConfigAttribute), true);
                if (configurableAttributes.Length == 1 && ((ConfigAttribute)configurableAttributes[0]).Configurable)
                {
                    string propertyName = propertyInfo.Name;
                    string attributeName = propertyName[0].ToString() + propertyName.Remove(0, 1);
                    string stringValue = textReader.GetAttribute(attributeName);
                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        object value;
                        if (propertyInfo.PropertyType.BaseType.FullName == "System.Enum")
                        {
                            value = Enum.Parse(propertyInfo.PropertyType, stringValue); 
                        }
                        else
                        {
                            value = Convert.ChangeType(stringValue, propertyInfo.PropertyType);
                        }
                        propertyInfo.SetValue(o, value, null);
                    }
                }
            }
        }
    }

    public class ConfigFileNamePropertyMissingException : DuradosException
    {
        public ConfigFileNamePropertyMissingException()
            : base("The config file name property is missing or empty.")
        {
        }
    }

    public class ConfigFileDoesNotExistException : DuradosException
    {
        public ConfigFileDoesNotExistException(string fileName)
            : base(string.Format("The config file does not exist at {0}.", fileName))
        {
        }
    }

    public class ReadingConfigFileException : DuradosException
    {
        public ReadingConfigFileException(Exception exception)
            : base("An error occurred during the reading of the configuration file.", exception)
        {
        }
    }

}
