using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZCMS.Core.Auth.Business;
using ZCMS.Core.Business;
using ZCMS.Core.Business.Utils;
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

        public ActionResult SignIn(string ReturnUrl)
        {
            ViewData["ReturnUrl"] = (!String.IsNullOrEmpty(ReturnUrl) ? ReturnUrl : String.Empty);

            return View(new ZCMSUser() { ReturnUrl = ViewData["ReturnUrl"].ToString()});
        }

        [HttpPost]
        public ActionResult SignIn(ZCMSUser user)
        {
            if (TryValidateModel(user))
            {
                if (_worker.AuthenticationRepository.AuthenticateUser(user.Username, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.Username, true);
                    if (!String.IsNullOrEmpty(user.ReturnUrl))
                        return Redirect(user.ReturnUrl);
                    else
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

        public ActionResult TwitterReturn()
        {
            HttpWebRequest hwr =
            (HttpWebRequest)HttpWebRequest.Create(
            @"https://api.twitter.com/oauth/authenticate?oauth_token=" + Request.QueryString["oauth_token"].ToString());

            hwr.Method = "GET";


            hwr.Timeout = 3 * 60 * 1000;

            WebResponse response = hwr.GetResponse();
            if (response.Headers["Status"].Equals("200 OK"))
            {
                // authorized, link to existing handle, or create and link
            }
            else
            {
            }

            // redirect back to the originating page.
            return Redirect(TempData["CurrentPageUrl"].ToString());
        }

        public ActionResult AuthorizeWithTwitter()
        {
            string redirectUrl = "/";
            TempData["CurrentPageUrl"] = ControllerContext.HttpContext.Request.UrlReferrer.AbsoluteUri;
            ZCMSGlobalConfig gc = ZCMSGlobalConfig.Instance;
            SocialService twitter;
            if (gc != null && gc.SocialServices != null && (twitter = gc.SocialServices.FirstOrDefault(f => f.ServiceName == "Twitter")) != null)
            {
                // see if it has tokens!
                if (String.IsNullOrEmpty(twitter.PublicToken))
                {
                    string tok = OAuthUtils.RequestNewTwitterToken(twitter);
                    string tokens = tok.Split(';')[0];

                    string oauthtoken;
                    string oauthsecret;
                    try
                    {
                        oauthtoken = tokens.Split('=')[1].Split('&')[0];
                        oauthsecret = tokens.Split('=')[2].Split('&')[0];
                        twitter.PublicToken = oauthtoken;
                        twitter.PrivateToken = oauthsecret;

                    }
                    catch
                    {
                    }
                }

                redirectUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + twitter.PublicToken;
            }
            return Redirect(redirectUrl);
        }
    }
}
