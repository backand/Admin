using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class FormType
    {
        public FormTypeEnum FormTypeEnum
        {
            get
            {
                return (FormTypeEnum)Enum.Parse(typeof(FormTypeEnum), this.Name);
            }
        }
    }

    public enum FormTypeEnum
    {
        DataRowView
    }
}
