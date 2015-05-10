using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Derivation
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public string DerivationField { get; set; }

        
        [Durados.Config.Attributes.ColumnProperty()]
        public string Deriveds { get; set; }

        public ParentField GetDerivationField(View view)
        {
            if (view.Fields.ContainsKey(DerivationField))
            {
                if (view.Fields[DerivationField].FieldType == FieldType.Parent)
                {
                    return (ParentField)view.Fields[DerivationField];
                }
            }

            return null;
        }
        private Dictionary<string, Dictionary<string, Field>> deriveds = null;
        private string prevDeriveds = null;

        //public Dictionary<string, Dictionary<string, Field>> GetDeriveds(View view)
        //{
        //    if (deriveds != null && prevDeriveds == this.Deriveds)
        //        return deriveds;

        //    deriveds = new Dictionary<string, Dictionary<string, Field>>();

        //    foreach (string derivedStr in view.Derivation.Deriveds.Split('|'))
        //    {
        //        string[] s = derivedStr.Split(';');
        //        string derivedValue = s[0].Trim('\n');
        //        deriveds.Add(derivedValue, new Dictionary<string,Field>());
        //        foreach (string field in s[1].Split(','))
        //        {
        //            string fieldName = field.Trim('\n');
        //            if (view.Fields.ContainsKey(fieldName))
        //            {
        //                deriveds[derivedValue].Add(fieldName, view.Fields[fieldName]);
        //            }
        //        }
        //    }
        //    prevDeriveds = this.Deriveds;

        //    return deriveds;
        //}

        public Dictionary<string, Dictionary<string, Field>> GetDeriveds(View view)
        {
            if (deriveds != null && prevDeriveds == this.Deriveds)
                return deriveds;

            deriveds = new Dictionary<string, Dictionary<string, Field>>();

            foreach (string derivedStr in view.Derivation.Deriveds.Split('|'))
            {
                string[] s = derivedStr.Split(';');
                string derivedValues = s[0].Trim('\n');
                Dictionary<string, Field> fields = new Dictionary<string, Field>();
                foreach (string field in s[1].Split(','))
                {
                    string fieldName = field.StripInvisibles();
                    if (view.Fields.ContainsKey(fieldName))
                    {
                        fields.Add(fieldName, view.Fields[fieldName]);
                    }
                }

                foreach (string derivedValue in derivedValues.Split('~'))
                {
                    deriveds.Add(derivedValue, fields);
                }
            }
            prevDeriveds = this.Deriveds;

            return deriveds;
        }

        public bool IsEditable(View view, Field field, System.Data.DataRow row)
        {
            ParentField derivationField = GetDerivationField(view);

            if (derivationField == null)
                return true;

            string derivedValue = derivationField.GetValue(row);

            if (derivedValue == null)
                return true;

            return IsEditable(view, field, derivationField, derivedValue);

        }

        public bool IsEditable(View view, Field field, ParentField derivationField, string derivedValue)
        {
            if (derivationField == null)
                return true;

            
            if (derivedValue == null)
                return true;

            Dictionary<string, Dictionary<string, Field>> deriveds = GetDeriveds(view);
            if (deriveds.ContainsKey(derivedValue))
            {
                if (deriveds[derivedValue].ContainsKey(field.Name))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class DerivationViolationException : DuradosException
    {
        public DerivationViolationException(Field field, System.Data.DataRow row) :
            base(GetMessage(field, row))
        {
        }

        private static string GetMessage(Field field, System.Data.DataRow row)
        {
            return "The column '" + field.DisplayName + "' of the '" + field.View.GetDisplayValue(row)+ "' row is currently in a read only mode.";
        }
    }
}
