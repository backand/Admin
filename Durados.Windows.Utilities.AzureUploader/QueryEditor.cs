using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Durados.Windows.Utilities.AzureUploader
{
    public class QueryEditor
    {
        public QueryEditor():this(UploadType.Config)
        {
        }
        public QueryEditor(UploadType uploadType)
        {
            QueryForm = new QueryForm();
            UploadType = uploadType;
            QueryForm.QueryEditor = this;
        }
        public QueryForm QueryForm { get; private set; }
        public DialogResult ShowDialog()
        {
            return QueryForm.ShowDialog();
        }


        public DataTable DataTable { get; set; }

        public UploadType UploadType {  get; private set; }
    }

    public enum UploadType
    {
        Config,
        Logo
    }
}
