using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Durados.Cms.Model
{
    public partial class Field
    {
        public object GetValue(object o)
        {
            Type type = o.GetType();

            PropertyInfo property = type.GetProperty(Name);

            if (IsScalar(property))
                return property.GetValue(o, null);
            else
            {
                object value = property.GetValue(o, null);

                if (value == null)
                    return null;

                Type valueType = value.GetType();

                PropertyInfo valueProperty = valueType.GetProperty(RelatedName);

                return valueProperty.GetValue(value, null);

            }
        }

        private bool IsScalar(PropertyInfo property)
        {
            object[] propertyAttributes = property.GetCustomAttributes(typeof(System.Data.Objects.DataClasses.EdmScalarPropertyAttribute), true);

            return propertyAttributes.Length == 1;
        }

        public string GetValueToString(object o, StringConverter stringConverter)
        {
            object value = GetValue(o);

            if (value == null)
                return string.Empty;

            return stringConverter.ConvertToString(value, o, this, Format);
        }

        

        public FieldCategory Category
        {
            get
            {
                if (!FieldCategoryRel.IsLoaded)
                    FieldCategoryRel.Load();

                if (FieldCategoryRel.Count > 0)
                {
                    FieldCategoryRel fieldCatecory = FieldCategoryRel.FirstOrDefault();
                    if (!fieldCatecory.FieldCategoryReference.IsLoaded)
                        fieldCatecory.FieldCategoryReference.Load();
                    return fieldCatecory.FieldCategory;
                }
                else
                {
                    return null;
                }

            }
        }

        public bool OnlyForRegistered
        {
            get
            {
                return !string.IsNullOrEmpty(Roles);
            }
        }

        public bool IsInRoles(string role)
        {
            return string.IsNullOrEmpty(this.Roles) || this.Roles.Contains(role);
            
        }

        public bool IsDisplayed(IEnumerable enumerable, DisplayFieldRules displayFieldRules, bool registered, string role)
        {
            if (!IsInRoles(role))
                return false;

            if (!registered && OnlyForRegistered)
                return false;

            foreach (object o in enumerable)
            {
                switch (displayFieldRules)
                {
                    case DisplayFieldRules.Always:
                        return true;
                    case DisplayFieldRules.AtLeastOne:
                        if (this.GetValue(o) != null)
                            return true;
                        break;
                    case DisplayFieldRules.MustAll:
                        if (this.GetValue(o) == null)
                            return false;
                        break;

                    default:
                        break;
                }
            }
            if (displayFieldRules == DisplayFieldRules.MustAll)
                return true;
            else // AtLeastOne
                return false;
        }

        public bool CanBeSorted
        {
            get
            {
                return BetterIsGreater.HasValue;
            }
        }

        public object GetBest(List<object> sortedValues)
        {
            if (sortedValues == null)
                return null;

            if (sortedValues.Count < 2)
                return null;

            if (BetterIsGreater.Value)
            {
                object last = sortedValues.Last();
                decimal lastValue = Convert.ToDecimal(GetValue(last));
                object beforeLast = sortedValues.ElementAt(sortedValues.Count - 2);
                decimal beforeLastValue = Convert.ToDecimal(GetValue(beforeLast));
                if (lastValue > beforeLastValue)
                    return last;
                else
                    return null;
            }
            else
            {
                object first = sortedValues.First();
                decimal firstValue = Convert.ToDecimal(GetValue(first));
                object afterFirst = sortedValues.ElementAt(1);
                decimal afterFirstValue = Convert.ToDecimal(GetValue(afterFirst));

                if (firstValue < afterFirstValue)
                    return first;
                else
                    return null;
            }
        }

        public object GetWorst(List<object> sortedValues)
        {
            if (sortedValues == null)
                return null;

            if (sortedValues.Count < 2)
                return null;

            if (!BetterIsGreater.Value)
            {
                object last = sortedValues.Last();
                decimal lastValue = Convert.ToDecimal(GetValue(last));
                object beforeLast = sortedValues.ElementAt(sortedValues.Count - 2);
                decimal beforeLastValue = Convert.ToDecimal(GetValue(beforeLast));
                if (lastValue > beforeLastValue)
                    return last;
                else
                    return null;
            }
            else
            {
                object first = sortedValues.First();
                decimal firstValue = Convert.ToDecimal(GetValue(first));
                object afterFirst = sortedValues.ElementAt(1);
                decimal afterFirstValue = Convert.ToDecimal(GetValue(afterFirst));

                if (firstValue < afterFirstValue)
                    return first;
                else
                    return null;
            }
        }

        public List<object> GetSortedValues(IEnumerable enumerable)
        {
            return GetSortedValues(enumerable, null);
        }

        public List<object> GetSortedValues(IEnumerable enumerable, ISpecificRules specificRules)
        {
            if (!CanBeSorted)
                return null;

            if (specificRules != null && specificRules.Overrides(this, enumerable))
                return specificRules.GetSortedValues(this, enumerable);

            List<object> values = new List<object>();
            foreach (object o in enumerable)
            {
                object value = this.GetValue(o);
                if (value!=null)
                    values.Add(o);
            }
            Comparer<object> comparer = new SortComparer<object>(this);
            values.Sort(comparer);

            return values;
        }

        public class SortComparer<T> : Comparer<T>
        {
            private Field field;

            public SortComparer(Field field)
            {
                this.field = field;
            }

            public override int Compare(T x, T y)
            {
                decimal d1 = Convert.ToDecimal(field.GetValue(x));
                decimal d2 = Convert.ToDecimal(field.GetValue(y));
                if (d1 > d2)
                    return 1;
                else if (d1 < d2)
                    return -1;
                else return 0;
            }
        }
    }

    public interface ISpecificRules
    {
        List<object> GetSortedValues(Field field, IEnumerable enumerable);

        bool Overrides(Field field, IEnumerable enumerable);
    }
}
