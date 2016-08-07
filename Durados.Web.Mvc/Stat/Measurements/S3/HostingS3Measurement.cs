using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Durados.DataAccess;
using System.Data;
using System.Data.SqlClient;


namespace Durados.Web.Mvc.Stat.Measurements.S3
{
    public class HostingS3Measurement : S3Measurement
    {
        public HostingS3Measurement(App app, MeasurementType measurementType)
            : base(app, measurementType)
        {

        }

        protected override string GetBucketName()
        {
            return Maps.S3Bucket; 
        }

        
        

        
    }
}
