using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZCMS.Core.Auth.Business;
using ZCMS.Core.Data;

namespace ZCMS.Core.Auth
{
    public class AuthenticationController : Controller
    {
        //
        // GET: /Authentication/
        UnitOfWork _worker;
        public AuthenticationController(UnitOfWork work)
        {
            _worker = work;
        }

        public ActionResult SignIn()
        {
            return View("SignIn");
        }

        [HttpPost]
        public ActionResult SignIn(ZCMSUser user)
        {
            if (TryValidateModel(user))
            {
                if (_worker.AuthenticationRepository.AuthenticateUser(user.Username, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.Username, true);
                    return RedirectToAction("Dashboard", new { Controller = "Backend", Action = "Dashboard" });
                }
                else
                {
                    ViewData["LogonFail"] = CMS_i18n.Auth.LogonFailedMessage;
                }
            }
            return View(user);
            
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("SignIn");
        }

    }
}
