using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZCMS.Core.Business;
using ZCMS.Core.Business.Utils;
using ZCMS.Core.Data;

namespace ZCMS.Core.Frontend.Controllers
{
    public class FrontendController : ZCMSBaseController
    {
        
        public FrontendController(UnitOfWork work):base(work) { }

        public ActionResult ViewPage(string slug)
        {
            var content = _worker.CmsContentRepository.GetCmsPageBySlug(slug);
            content.SocialServices = new Business.ZCMSSocial(_worker.CmsContentRepository.GetSocialServiceConfigs());
            content.SiteDescription = _worker.CmsContentRepository.GetSiteDescription();
            if (content.Instance == null && content.ViewStatus == Business.ContentViewStatus.NotAuthenticated)
                return RedirectToAction("signin", "authentication", new { ReturnUrl = "/pages/"+content.GetMetadataValue("RedirectUrl") });
            else
                return View(content);
        }

        public ActionResult TwitterReturn()
        {
            ServicePointManager.Expect100Continue = false;
            var conf = ZCMSGlobalConfig.Instance;
            var tw = conf.SocialServices.Where(s => s.ServiceName == "Twitter").FirstOrDefault();
            HttpWebRequest hwr =
            (HttpWebRequest)HttpWebRequest.Create(
            @"https://api.twitter.com/1.1/statuses/home_timeline.json?" + OAuthUtils.GetAuthHeader("https://api.twitter.com/1.1/statuses/home_timeline.json", "GET", tw.Key, tw.Secret, Request.QueryString["oauth_token"].ToString()).Split(';')[1].Replace("[TOKEN]", Request.QueryString["oauth_token"].ToString()));

            hwr.Method = "GET";
            //hwr.Headers.Add("Authorization", OAuthUtils.GetAuthHeader("https://api.twitter.com/1/statuses/home_timeline.json", twitter.Key, twitter.Secret));
            //string sh = hwr.Headers["Authorization"].ToString();
            //hwr.ContentType = "application/x-www-form-urlencoded";


            hwr.Timeout = 3 * 60 * 1000;

            WebResponse wresponse = hwr.GetResponse();
            StreamReader reader = new StreamReader(wresponse.GetResponseStream());
            string responseString = reader.ReadToEnd();
            reader.Close();


            return RedirectToAction("ViewPage", new { slug = "signedin" });
        }

    }
}
