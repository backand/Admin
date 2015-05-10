using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class Contact
    {
        const char Seperator = ';';
        public string[] ToMails
        {
            get
            {
                return To.Split(Seperator);
            }
        }

        public string[] CcMails
        {
            get
            {
                return CC.Split(Seperator);
            }
        }
    }
}
