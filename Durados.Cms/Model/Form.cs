using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class Form
    {
        public FormTypeEnum FormTypeEnum
        {
            get
            {
                if (!FormTypeReference.IsLoaded)
                    FormTypeReference.Load();
                return FormType.FormTypeEnum;
            }
        }

        public Durados.DataAction DataActionEnum
        {
            get
            {
                return (Durados.DataAction)Enum.Parse(typeof(Durados.DataAction), DataAction);
            }
        }
    }
}
