using Durados.Data;
using Durados.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BackAnd.Web.Api.Controllers.Admin
{
    [RoutePrefix("1")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize("Developer")]
    public class FarmController : ApiController
    {
        [Route("~/1/farm")]
        public IHttpActionResult Get()
        {
            var status = Maps.Instance.StorageCache as IStatusCache;

            if (status == null)
            {
                return BadRequest("Maps is not IStatusCache");
            }

            return Ok(status.GetCacheStatus());
        }
    }
}
