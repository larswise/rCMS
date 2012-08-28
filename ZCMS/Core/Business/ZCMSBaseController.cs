using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZCMS.Core.Data;
using ZCMS.Core.Security;

namespace ZCMS.Core.Business
{
    [ZCMSAuthenticate(Roles = "Administrators")]
    public abstract class ZCMSBaseController : Controller
    {
        public ZCMSMenu Model { get; set; }
        protected UnitOfWork _worker;
        protected List<ZCMSMenu> _menu;

        public ZCMSBaseController(UnitOfWork work, List<ZCMSMenu> menu)
        {
            _worker = work;
            _menu = menu;

            ViewData["MenuData"] = _menu;
        }

        public ZCMSBaseController(UnitOfWork work)
        {
            _worker = work;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _worker.OpenSession();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            _worker.SaveAllChanges();
            _worker.CloseSession();
        }

    }
}