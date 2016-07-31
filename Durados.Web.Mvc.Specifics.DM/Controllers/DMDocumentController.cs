using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Specifics.DM.Controllers
{
    public class DMDocumentController : Durados.Web.Mvc.Controllers.CrmController
    {
        protected override Durados.Web.Mvc.UI.ImageViewerInfo GetImageUrl(string viewName, string pk)
        {

            ImageViewerInfo iv = new ImageViewerInfo();
            View view = GetView(viewName);
            DataRow dataRow = view.GetDataRow(pk);

            iv.Url = ((ColumnField)view.Fields["DocPath"]).Upload.UploadVirtualPath + ((ColumnField)view.Fields["DocPath"]).GetValue(dataRow);

            return iv;
            
        }        
        
    }

}
