using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZCMS.Core.Data;

namespace ZCMS.Core.Frontend.Controllers
{
    public class FrontendController : Controller
    {
        private UnitOfWork _worker;
        public FrontendController(UnitOfWork work)
        {
            _worker = work;
        }

        public ActionResult ViewPage(string slug)
        {
            var content = _worker.CmsContentRepository.GetCmsPageBySlug(slug);
            if (content.Instance == null && content.ViewStatus == Business.ContentViewStatus.NotAuthenticated)
                return RedirectToAction("signin", "authentication", new { ReturnUrl = "/pages/"+content.GetMetadataValue("RedirectUrl") });
            else
                return View(content);
        }

    }
}
