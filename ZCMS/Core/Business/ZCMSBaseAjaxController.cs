using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using ZCMS.Core.Data;

namespace ZCMS.Core.Business
{
    public class ZCMSBaseAjaxController : ApiController
    {
        protected UnitOfWork _worker;

        public ZCMSBaseAjaxController(UnitOfWork work)
        {
            _worker = work;            
        }

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            _worker.OpenSession();
        }

        protected override void Dispose(bool disposing)
        {
            _worker.SaveAllChanges();
            _worker.CloseSession();
            base.Dispose(disposing);
        }
    }
}