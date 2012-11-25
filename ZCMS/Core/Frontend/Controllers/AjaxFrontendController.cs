using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Security;
using Facebook;
using ZCMS.Core.Business;
using ZCMS.Core.Business.Utils;
using ZCMS.Core.Data;

namespace ZCMS.Core.Frontend.Controllers
{
    public class AjaxFrontendController : ZCMSBaseAjaxController
    {
        public AjaxFrontendController(UnitOfWork worker)
            : base(worker)
        {
           
        }

        public string GetSocialLogin(string token)
        {
            System.Web.HttpContext.Current.Session["AccessToken"] = token;
            var client = new FacebookClient(token);
            dynamic result = client.Get("me", new { fields = "name,id" });
            string name = result.name;
            FormsAuthentication.SetAuthCookie(name, true);
            return result.id;
        }

        public HttpResponseMessage GetTwitterSigningKey(string consumerKey)
        {
            var response = Request.CreateResponse(HttpStatusCode.Moved);

            ZCMSGlobalConfig gc = ZCMSGlobalConfig.Instance;
            SocialService twitter;
            if (gc != null && gc.SocialServices != null && (twitter = gc.SocialServices.FirstOrDefault(f => f.ServiceName == "Twitter"))!=null)
            {
                // see if it has tokens!
                if (String.IsNullOrEmpty(twitter.PublicToken))
                {
                    string tokens = OAuthUtils.RequestNewTwitterToken(twitter);
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
                response.Headers.Location = new Uri("https://api.twitter.com/oauth/authenticate?oauth_token=" + twitter.PublicToken);
            }
            return response;
        }
    }
}
