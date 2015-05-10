using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class Filter
    {
        private Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        [DataMember]
        public Dictionary<string, Field> Fields { get; private set; }

        public Filter()
        {
            Fields = new Dictionary<string, Field>();
        }

        public override string ToString()
        {
            string s = string.Empty;

            foreach (Field field in Fields.Values)
            {
                s += field.Name;
                s += "=";
                s += field.Value.ToString();
                s += " and ";
            }

            return s.TrimEnd(" and ".ToCharArray());
            
        }

        public virtual string GetParentFiterValue()
        {
            foreach (Field field in Fields.Values)
            {
                return field.Value.ToString();
            }

            return string.Empty;
        }

        public virtual string GetParentFiterFieldName()
        {
            string name = GetParentFiterFieldName(Map.Database);

            if (string.IsNullOrEmpty(name))
                name = GetParentFiterFieldName(Map.GetConfigDatabase());

            return name;
        }

        public virtual string GetParentFiterFieldName(Database database)
        {
            Durados.ParentField parentField = null;
            foreach (Field field in Fields.Values)
            {
                foreach (Durados.View view in database.Views.Values)
                {
                    if (view.Fields.ContainsKey(field.Name))
                    {
                        Durados.Field f = view.Fields[field.Name];
                        switch (f.FieldType)
                        {
                            case FieldType.Parent:
                                break;

                            case FieldType.Children:
                                parentField = ((ChildrenField)f).GetRelatedParentField();
                                break;

                            case FieldType.Column:
                                return field.Name;

                            default:
                                throw new NotSupportedException();
                        }
                        break;
                    }
                }
            }

            if (parentField == null)
                return string.Empty;

            return parentField.Name;
        }

        public virtual string ToFriendlyString()
        {
            string s = string.Empty;

            foreach (Field field in Fields.Values)
            {
                string displayName = string.Empty;
                foreach (Durados.View view in Map.Database.Views.Values)
                {
                    if (view.Fields.ContainsKey(field.Name))
                    {
                        Durados.Field f = view.Fields[field.Name];
                        switch (f.FieldType)
                        {
                            case FieldType.Parent:
                                displayName = ((Durados.ParentField)f).DisplayName;
                                break;

                            case FieldType.Children:
                                displayName = ((ChildrenField)f).GetRelatedParentField().DisplayName;
                                
                                
                                break;

                            default:
                                throw new NotSupportedException();
                        }
                        break;
                    }
                }
                if (displayName ==string.Empty)
                    displayName = field.Name;
                s += displayName;
                s += "=";
                s += field.Value.ToString();
                s += " and ";
            }

            return s.EndsWith("and ") ? s.Remove(s.Length - 5) : s;

        }
    }
}
