using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public class StringConverter
    {
        public StringConverter() { }

        public virtual string ConvertToString(object value, object o, Field field, string format)
        {
            Type type = value.GetType();

            string s;

            if (value == null || value.Equals(DBNull.Value))
            {
                s = string.Empty;
            }
            else if (type.Equals(typeof(Int32)))
            {
                s = Convert.ToInt32(value).ToString(format);
            }
            else if (type.Equals(typeof(Int16)))
            {
                s = Convert.ToInt16(value).ToString(format);
            }
            else if (type.Equals(typeof(Int64)))
            {
                s = Convert.ToInt64(value).ToString(format);
            }
            else if (type.Equals(typeof(decimal)))
            {
                s = Convert.ToDecimal(value).ToString(format);
            }
            else if (type.Equals(typeof(Double)))
            {
                s = Convert.ToDouble(value).ToString(format);
            }
            else if (type.Equals(typeof(Single)))
            {
                s = Convert.ToSingle(value).ToString(format);
            }
            else if (type.Equals(typeof(Byte)))
            {
                s = Convert.ToByte(value).ToString(format);
            }
            else if (type.Equals(typeof(Boolean)))
            {
                s = value.ToString();
            }
            else if (type.Equals(typeof(DateTime)))
            {
                s = Convert.ToDateTime(value).ToString(format);
            }
            else
            {
                s = value.ToString();
            }

            return s;
        }
    }
}
