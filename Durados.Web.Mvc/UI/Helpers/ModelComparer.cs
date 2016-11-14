using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class ModelComparer
    {
        public bool IsEquals(ArrayList oldModel, ArrayList newModel)
        {
            if (!(oldModel.Count.Equals(newModel.Count)))
            {
                return false;
            }

            for (int i = 0; i < oldModel.Count; i++)
            {
                if (!IsOldOjectEqualsToCurrent((Dictionary<string, object>)oldModel[i], (Dictionary<string, object>)newModel[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsOldOjectEqualsToCurrent(Dictionary<string, object> oldObject, Dictionary<string, object> newObject)
        {
            const string Name = "name";
            const string Fields = "fields";

            if (!(oldObject.ContainsKey(Name) && newObject.ContainsKey(Name)))
            {
                return false;
            }
            if (!oldObject[Name].Equals(newObject[Name]))
            {
                return false;
            }
            if (!(oldObject.ContainsKey(Fields) && newObject.ContainsKey(Fields)))
            {
                return false;
            }

            Dictionary<string, object> oldObjectFields = (Dictionary<string, object>)oldObject[Fields];
            Dictionary<string, object> newObjectFields = (Dictionary<string, object>)newObject[Fields];
            if (!(oldObjectFields.Count.Equals(newObjectFields.Count)))
            {
                return false;
            }

            foreach (string key in oldObjectFields.Keys)
            {
                if (!(oldObjectFields.ContainsKey(key) && newObjectFields.ContainsKey(key)))
                {
                    return false;
                }
                if (!IsOldFieldEqualsToCurrent((Dictionary<string, object>)oldObjectFields[key], (Dictionary<string, object>)newObjectFields[key]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsOldFieldEqualsToCurrent(Dictionary<string, object> oldField, Dictionary<string, object> newField)
        {
            foreach (string key in oldField.Keys)
            {
                if (!(oldField.ContainsKey(key) && newField.ContainsKey(key)))
                {
                    return false;
                }
                if (key == "defaultValue")
                {
                    if (newField[key] is decimal)
                    {
                        if (!Convert.ToDecimal(oldField[key]).Equals(newField[key]))
                        {
                            return false;
                        }
                    }
                }
                else if (!oldField[key].Equals(newField[key]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
