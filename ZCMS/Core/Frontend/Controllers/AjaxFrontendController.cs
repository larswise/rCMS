using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Security;
using Facebook;
using ZCMS.Core.Business;
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

        public string GetTwitterSigningKey(string consumerKey)
        {
            string hash = string.Empty;

            var socialConfig = _worker.CmsContentRepository.GetSocialServiceConfigs().Where(s => s.ServiceName == "Twitter").FirstOrDefault();

            if (socialConfig != null)
            {
                string signingKey =
                    Uri.EscapeDataString(socialConfig.Secret) + "&" +
                    Uri.EscapeDataString(socialConfig.PrivateToken);

                using (HMACSHA1 hmac = new HMACSHA1())
                {
                    hash = Convert.ToBase64String(
                        hmac.ComputeHash(
                        new ASCIIEncoding().GetBytes(signingKey)));
                }
            }
            return hash;
         }
    }
}
