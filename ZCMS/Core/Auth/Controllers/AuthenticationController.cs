using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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
        public ActionResult SignIn(FormCollection coll)
        {
            string pass = coll["pass"].ToString();
            string user = coll["username"].ToString();

            if (_worker.AuthenticationRepository.AuthenticateUser(user, pass))
            {
                FormsAuthentication.SetAuthCookie(user, true);
                return RedirectToAction("PageEditor", "Backend");
            }
            else
            {
                ViewData["LogonFail"] = CMS_i18n.Auth.LogonFailedMessage;
                return View(ViewData["LogonFailed"]);
            }
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("SignIn");
        }

    }
}
