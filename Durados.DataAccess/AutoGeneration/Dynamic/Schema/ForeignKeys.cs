using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration.Dynamic.Schema
{
    public class ForeignKeys
    {
        private Dictionary<string, ForeignKey> foreignKeyNameDictionary = new Dictionary<string, ForeignKey>();
        private Dictionary<string, ForeignKey> columnsNamesDictionary = new Dictionary<string, ForeignKey>();

        public ForeignKey this[string columnsNames]
        {
            get
            {
                return GetByColumnsNames(columnsNames);
            }
        }

        public ForeignKey GetByForeignKeyName(string foreignKeyName)
        {
            return foreignKeyNameDictionary[foreignKeyName];
        }

        public ForeignKey GetByColumnsNames(string columnsNames)
        {
            return columnsNamesDictionary[columnsNames];
        }

        public bool ContainsByForeignKeyName(string foreignKeyName)
        {
            return foreignKeyNameDictionary.ContainsKey(foreignKeyName);
        }

        public bool ContainsByColumnsNames(string columnsNames)
        {
            return columnsNamesDictionary.ContainsKey(columnsNames);
        }

        public void Add(ForeignKey foreignKey)
        {
            string columnsNames = foreignKey.ColumnsNames.ToArray().Delimited();

            if (foreignKeyNameDictionary.ContainsKey(foreignKey.Name))
            {
                throw new DuradosException("The View already contains the foreign key " + foreignKey.Name);
            }


            if (columnsNamesDictionary.ContainsKey(columnsNames))
            {
                throw new DuradosException("The View already contains the foreign key with columns " + columnsNames);
            }

            foreignKeyNameDictionary.Add(foreignKey.Name, foreignKey);

            columnsNamesDictionary.Add(columnsNames, foreignKey);

        }

    }
}
