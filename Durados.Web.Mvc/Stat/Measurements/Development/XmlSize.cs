using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Stat.Measurements.Development
{
    public class XmlSize : DevMeasurement
    {
        public XmlSize(App app, MeasurementType measurementType)
            : base(app, measurementType)
        {

        }

        public override object Get(DateTime date)
        {
            var appRow = GetAppRow();

            var id = appRow.Id.ToString();

            var value = GetSpaceUsed(id);

            return value;
        }

        public static long GetSpaceUsed(string id)
        {
            string containerName = Maps.DuradosAppPrefix.Replace("_", "").Replace(".", "").ToLower() + id.ToString();

            Azure.DuradosStorage storage = new Azure.DuradosStorage();

            CloudBlobContainer container = storage.GetContainer(containerName);

            var listBlobs = container.ListBlobs();

            var blob = (CloudBlob)listBlobs.Where(b => ((CloudBlob)b).Name == containerName).FirstOrDefault();

            if (blob == null)
            {
                throw new DuradosException("Missing xml");
            }
            return blob.Properties.Length;
        }
    }
}
