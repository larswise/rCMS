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

        public ZCMSBaseAjaxController()
        {
        }

        public ZCMSBaseAjaxController(UnitOfWork work)
        {
            _worker = work;            
        }

    }
}