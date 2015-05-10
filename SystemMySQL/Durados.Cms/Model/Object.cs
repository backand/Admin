using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Durados.Cms.Model
{
    public partial class Object
    {

        public void UpdateCms(Type type)
        {
            UpdateCms(type, false);
        }

        public void UpdateCms(Type type, bool justScalars)
        {
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object[] propertyAttributes;
                if (justScalars)
                    propertyAttributes = property.GetCustomAttributes(typeof(System.Data.Objects.DataClasses.EdmScalarPropertyAttribute), true);
                else
                    propertyAttributes = property.GetCustomAttributes(typeof(System.Runtime.Serialization.DataMemberAttribute), true);

                if (propertyAttributes.Length == 1)
                {
                    try
                    {
                        Durados.Cms.Singleton.Cms.AddToFieldSet(new Durados.Cms.Model.Field() { Object = this, DisplayName = property.Name, Name = property.Name });
                        Durados.Cms.Singleton.Cms.SaveChanges();
                    }
                    catch { }
                }
            }
        }
    }
}
