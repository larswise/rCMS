using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Raven.Client;
using ZCMS.Core.Auth.Business;
using ZCMS.Core.Business;
using ZCMS.Core.Business.Content;
using ZCMS.Core.Business.Validators;
using ZCMS.Core.Data;
using ZCMS.Core.Security;
using ZCMS.Core.Utils;


namespace ZCMS.Core.Backend.Controllers
{    
    public class BackendController : ZCMSBaseController
    {
        
        public BackendController(UnitOfWork worker, List<ZCMSMenu> menu):base(worker, menu) { }

        public ActionResult AdminStart()
        {
            return View();
        }

        #region Page Actions
        public ActionResult PageEditor(int ?pageId, string mParameter)
        {
            ZCMSContent<ZCMSPage> page;
            dynamic pagePublishType = ZCMSPageFactory.GetPagePublishType(mParameter);
            
            if (pageId.HasValue)
            {
                ViewData["CurrentPageId"] = pageId.HasValue ? pageId.Value : new Random().Next();
                page = _worker.CmsContentRepository.GetCmsPage(pageId.ToString());
                ViewData["PermissionSet"] = PermissionSet.GetAvailablePermissions(page.GetMetadataValue("Raven-Document-Authorization")).Select(x => new SelectListItem { Value = x.PermissionValue, Text = x.PermissionDisplay, Selected = x.Selected });
                                
                if (page.Instance != null)
                {
                    TempData["ImageCollection"] = _worker.GetAttachments(page.Instance).Select(w => new WebImage(w)).ToList();
                }
                else
                {
                    page = new ZCMSContent<ZCMSPage>(new ZCMSPage(pagePublishType) { Status = PageStatus.New, PageID = Int32.Parse(ViewData["CurrentPageId"].ToString()) });
                }
            }
            else
            {
                ViewData["PermissionSet"] = PermissionSet.GetAvailablePermissions(string.Empty).Select(x => new SelectListItem { Value = x.PermissionValue, Text = x.PermissionDisplay, Selected = x.Selected });

                page = new ZCMSContent<ZCMSPage>(new ZCMSPage(pagePublishType) { Status = PageStatus.New, PageID = new Random().Next() });
                ViewData["CurrentPageId"] = page.Instance.PageID;
            }
            page.AllTopics = _worker.CmsContentRepository.GetTopics().Topics.OrderBy(o => o.Name).ToList();
            page.AllTopics.Add(new ZCMSTopic() { TopicId = 0, Name = CMS_i18n.BackendResources.TopicsNoneSelected, Color = "" });
            return View(page);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PageEditor(ZCMSContent<ZCMSPage> page)
        {
            if (Request.Form["delete-page"] != null && Request.Form["delete-page"] == "true")
            {
                _worker.CmsContentRepository.DeleteCmsPage(page.Instance);
                return RedirectToAction("DashBoard");
            }

            TryValidateModel(page.Instance);
            //var validato = new ZCMSModelValidatorProvider();
            //validato.GetValidators(
            ZCMSPage ravenPage = _worker.CmsContentRepository.GetCmsPage(page.Instance.PageID.ToString()).Instance;
            if (ModelState.IsValid)
            {
                if (ravenPage != null)
                {
                    ravenPage.PageID = page.Instance.PageID;
                    ravenPage.PageName = page.Instance.PageName;
                    _worker.CmsContentRepository.SetPermissionsForPage(ravenPage, Request.Form["Permissions"].ToString());
                    for (int i = 0; i < ravenPage.Properties.Count; i++)
                    {
                        if (!(ravenPage.Properties[i] is ImageListProperty))
                            ravenPage.Properties[i] = page.Instance.Properties[i];                        
                    }
                                        
                    ravenPage.LastModified = DateTime.Now;
                    ravenPage.LastChangedBy = _worker.AuthenticationRepository.GetCurrentUserName();
                    ravenPage.Status = page.Instance.Status;
                    ravenPage.StartPublish = page.Instance.StartPublish;
                    ravenPage.EndPublish = page.Instance.EndPublish;
                    ravenPage.TopicId = page.Instance.TopicId;
                    TempData["DocumentSaved"] = CMS_i18n.BackendResources.DocumentSaved;
                    return RedirectToAction("PageEditor", new { pageId = ravenPage.PageID });
                }
                else
                {
                    page.Instance.WrittenBy = _worker.AuthenticationRepository.GetCurrentUserName();
                    page.Instance.LastChangedBy = page.Instance.WrittenBy;
                    page.Instance.LastModified = DateTime.Now;
                    page.Instance.Created = DateTime.Now;
                    _worker.CmsContentRepository.CreateCmsPage(page.Instance, Request.Form["Permissions"].ToString());
                }
                return RedirectToAction("PageEditor", new { pageId = page.Instance.PageID });
            }
            else
            {
                ViewData["PermissionSet"] = PermissionSet.GetAvailablePermissions(string.Empty).Select(x => new SelectListItem { Value = x.PermissionValue, Text = x.PermissionDisplay, Selected = x.Selected });
                ViewData["CurrentPageId"] = page.Instance.PageID;
                return View(new ZCMSContent<ZCMSPage>(ravenPage == null ? page.Instance : ravenPage));
            }
        }

        public ActionResult Social()
        { 
            ZCMSSocial zSocial = new ZCMSSocial(_worker.CmsContentRepository.GetSocialServiceConfigs());            
            return View(zSocial);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Social(ZCMSSocial social)
        {
            social.Facebook.ServiceName = "Facebook";
            social.Twitter.ServiceName = "Twitter";
            social.Disqus.ServiceName = "Disqus";
            _worker.CmsContentRepository.SaveSocialConfigs(social);
            return RedirectToAction("Social");
        }

        public ActionResult Topics()
        {
            ZCMSTopics zTopics = _worker.CmsContentRepository.GetTopics();
            return View(zTopics);
        }

        [HttpPost]
        public ActionResult Topics(ZCMSTopics topics)
        {
            _worker.CmsContentRepository.SaveTopics(topics);
            return RedirectToAction("Topics");
        }

        public ActionResult SiteDescription()
        {
            ZCMSSiteDescription description = _worker.CmsContentRepository.GetSiteDescription();
            return View(description);
        }

        [HttpPost]
        public ActionResult SiteDescription(ZCMSSiteDescription description)
        {
            _worker.CmsContentRepository.SaveSiteDescription(description);
            return RedirectToAction("SiteDescription");
        }

        public ActionResult PageTypeEditor(string mParameter)
        {
            ZCMSPageTypes typesViewModel = new ZCMSPageTypes() { PageTypes = _worker.CmsContentRepository.GetPageTypes() };
            return View(typesViewModel);
        }

        public ActionResult Dashboard()
        {
            return View("Dashboard", new ZCMSDashBoard() { Topics = _worker.CmsContentRepository.GetTopics().Topics  });
        }

        public ActionResult FileManager()
        {
            return View(new ZCMSFileManager(_worker.FileRepository.GetAllFileTypes(), null, DateTime.Now.AddDays(-10)) 
                            {
                                FileDocuments = _worker.FileRepository.QueryAttachment(new List<string>() { "*" }, string.Empty)
                            });
        }

        public ActionResult Revise(string id)
        {
            string[] pageIdArr = id.Split('.');
            string actual = pageIdArr[0];

            ZCMSPage pg = _worker.CmsContentRepository.GetCmsPage(actual).Instance;
            ZCMSPage pgrev = _worker.CmsContentRepository.GetCmsPage(id.Replace(".", "/")).Instance;
            pg.Properties = pgrev.Properties;
            
            pg.PageName = pgrev.PageName;
            

            return RedirectToAction("PageEditor", new { pageId = pg.PageID });
        }
        #endregion

        #region Ajax Helper Methods


        public string GetPages(string key)
        {
            var items = _worker.CmsContentRepository.GetRecentPages(null, 12)
                .Select(z => 
                    new 
                    { 
                        PageName = z.PageName, 
                        PageId = z.PageID,  
                        Created = z.Created.ToString(CMS_i18n.Formats.DateFormat),
                        LastModified = z.LastModified.ToString(CMS_i18n.Formats.DateFormat),
                        CreatedBy = z.WrittenBy,
                        LastModifiedBy = z.LastChangedBy,
                        Status = z.Status.ToString(),
                        StartPublish = z.StartPublish,
                        EndPublish = z.EndPublish,
                        PageType = z.PageType,                       
                        EditUrl = "/"+((ZCMSApplication)HttpContext.ApplicationInstance).MainAdminUrl+"/PageEditor/"+z.PageID,
                        ViewUrl = "/"+((ZCMSApplication)HttpContext.ApplicationInstance).MainContentUrl+"/"+z.SlugValue
                    });
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(items);
        }


        public string GetTopics()
        {
            var items = _worker.CmsContentRepository.GetTopics();
            if (items!= null && items.Topics != null && items.Topics.Count > 0)
            {
                var topics = items.Topics
                   .Select(z =>
                       new
                       {
                           Name = z.Name,
                           Description = z.Description,
                           Color = z.Color,
                           ShowInMenu = z.ShowInMenu,
                           TopicId = z.TopicId
                       });
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(topics);
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region Partial view rendering

        public ActionResult RenderAllRevisions(string id)
        {
            try
            {
                var items = _worker.CmsContentRepository.GetPastRevisions(id);
                if (items.Count > 0)
                    return PartialView("RenderAllRevisions", _worker.CmsContentRepository.GetPastRevisions(id));
                else
                    return PartialView("RenderAllRevisions", new List<ZCMSMetaData>());
            }
            catch
            {
                return PartialView();
            }
        }
        #endregion

    }
}
