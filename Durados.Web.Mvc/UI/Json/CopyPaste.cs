using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class CopyPaste
    {

        [DataMember]
        public SourceObjects Source { get; set; }

        [DataMember]
        public DestinationObjects Destination { get; set; }

        public CopyPaste()
        {
            this.Source = new SourceObjects();
            this.Destination = new DestinationObjects();
        }

        [DataContract]
        public class SourceObjects
        {
            [DataMember]
            public List<String> FieldsNames { get; set; }

            [DataMember]
            public List<List<String>> FieldsValues { get; set; }

            public SourceObjects()
            {
                this.FieldsNames = new List<string>();
                this.FieldsValues = new List<List<string>>();
            }
        }

        [DataContract]
        public class DestinationObjects
        {
            [DataMember]
            public List<String> RowsPKs { get; set; }

            public DestinationObjects()
            {
                this.RowsPKs = new List<string>();
            }
        }

        public int GetRowsNumber()
        {
            int values = this.Source.FieldsValues.Count;
            int pks = this.Destination.RowsPKs.Count;

            if ( pks % values != 0 )
            {
                throw new DuradosException("Source values doesn't match Destination rows number");
            }

            return pks;

        }
        

        public Dictionary<string, object> GetNameValuesOfRow(int key)
        {

            //validate
            int names = this.Source.FieldsNames.Count;

            if (names != this.Source.FieldsValues[key].Count)
            {
                throw new DuradosException("Source values do not match Source Fields Names");
            }


            Dictionary<string, object> values = new Dictionary<string, object>();

            for (int i = 0; i < names; i++)
            {
                values.Add(this.Source.FieldsNames[i], this.Source.FieldsValues[key][i]);
            }

            return values;
        }

        public List<Dictionary<string, object>> GetNameValuesOfRows()
        {
            List<Dictionary<string, object>> Rows = new List<Dictionary<string, object>>();

            Dictionary<string, object> values = new Dictionary<string, object>();

            int names = this.Source.FieldsNames.Count;

            for (int k = 0; k < names; k++)
            {
                //validate
                if (names != this.Source.FieldsValues[k].Count)
                {
                    throw new DuradosException("Source values do not match Source Fields Names");
                }

                for (int i = 0; i < this.Source.FieldsNames.Count; i++)
                {
                    values.Add(this.Source.FieldsNames[i], this.Source.FieldsValues[k][i]);
                }
            }

            Rows.Add(values);

            return Rows;
        }
        
    }

}