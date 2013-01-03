using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZCMS.Core.Business;
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
            string tokentodostuffwith = Request.QueryString["oauth_token"].ToString();

            // we can use the token to get the user home timeline etc...


            return RedirectToAction("ViewPage", new { slug = "signedin" });
        }

    }
}
