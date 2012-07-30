using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZCMS.Core.Data;

namespace ZCMS.Core.Business
{
    public class ZCMSBaseController : Controller
    {
        public ZCMSMenu Model { get; set; }
        protected UnitOfWork _worker;
        public ZCMSBaseController(UnitOfWork work)
        {
            _worker = work;

            if (System.Web.HttpContext.Current.Request.Cookies["active-menu"]!=null)
                ViewData["MenuData"] = _worker.CmsContentRepository.GetMenu(System.Web.HttpContext.Current.Request.Cookies["active-menu"].Value.ToString());
        } 

    }
}