using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration.Dynamic.Schema
{
    public class Dependency
    {
        public string ObjectName { get; set; }
        public string Definition { get; set; }
        
        // key = object name, value = list of columns names
        public Dictionary<string, List<string>> DependentColumns { get; private set; }
        
        readonly char POINT = '.';
        readonly char SPACE = ' ';

        public Dependency()
        {
            DependentColumns = new Dictionary<string, List<string>>();
        }

        private string GetStandardDefinition()
        {

            return Definition.ToLower().StripInvisibles().StripBreaks();
        }

        public string GetOriginalName(string columnName)
        {
            if (columnName.Contains(' '))
                return null;

            string definition = GetStandardDefinition();

            if (definition.CountOccurrences(columnName) != 1)
                return null;

            string fullName = GetCurrentOriginalFullName(definition, columnName);
            string objectName = GetObjectName(fullName);
            string currentOriginalColumnName = GetColumnName(fullName);



            Dependency dependency = GetDependency(objectName, currentOriginalColumnName);

            if (dependency == null)
                return null;

            return dependency.GetOriginalName(currentOriginalColumnName);


        }


        private Dependency GetDependency(string objectName, string columnName)
        {
            //if (DependentColumns.ContainsKey(objectName))
            //{
            //    if (DependentColumns[objectName].Contains(columnName))
            //        if (Dependencies.ContainsKey(objectName))
            //            return Dependencies[objectName];
            //}
            return null;
        }

        private string GetColumnName(string fullName)
        {
            string[] arr = fullName.Split('.');
            if (arr.Count() != 2)
                return null;
            return arr[1];
        }

        private string GetObjectName(string fullName)
        {
            string[] arr = fullName.Split('.');
            if (arr.Count() != 2)
                return null;
            return arr[0];
        }

        private string GetCurrentOriginalFullName(string definition, string columnName)
        {
            char[] chars = columnName.ToLower().ToCharArray();
            int index = definition.IndexOfAny(chars);

            if (IsAlias(definition, columnName))
            {
                index = index - 4;

                if (definition[index] != SPACE)
                {
                    return null;
                }

                int start = 0;
                string originalColumnName = definition.GetLastWordUpToHere(index, out start, new char[2] { POINT, SPACE });

                index = start;
                index--;
                if (definition[index] != POINT)
                    return null;

                string objectName = definition.GetLastWordUpToHere(index, out start, new char[2] { POINT, SPACE });

                return objectName + POINT + originalColumnName;

            }
            else
            {
                index--;

                if (definition[index] != POINT)
                {
                    return null;
                }

                int start = 0;

                string objectName = definition.GetLastWordUpToHere(index, out start, new char[2] { POINT, SPACE });

                return objectName + POINT + columnName;
            }
        }

        private bool IsAlias(string definition, string columnName)
        {
            char[] chars = columnName.ToLower().ToCharArray();
            int index = definition.IndexOfAny(chars);

            return definition.Substring(index - 4, index).Equals(" as ");
        }
    }

    
}
