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
            dynamic pagePublishType = ZCMSPageFactory.GetPagePublishType(mParameter);

            ViewData["PermissionSet"] = PermissionSet.GetAvailablePermissions().Select(x => new SelectListItem { Value = x.PermissionValue, Text = x.PermissionDisplay });
                
            if (pageId.HasValue)
            {

                ViewData["CurrentPageId"] = pageId.HasValue ? pageId.Value : new Random().Next();
                ZCMSPage page = _worker.CmsContentRepository.GetCmsPage(pageId.ToString());

                if (page != null)
                {
                    List<WebImage> mses = new List<WebImage>();
                    if (page.Properties.Where(p => p is ImageListProperty).Any())
                    {
                        var prop = page.Properties.Where(p => p is ImageListProperty).FirstOrDefault();
                        List<string> images = (List<string>)prop.PropertyValue;
                        foreach (var item in images)
                        {
                            mses.Add(new WebImage(_worker.FileRepository.RetrieveAttachment(item)));
                        }
                        TempData["ImageCollection"] = mses;
                    }

 
                    return View(page);
                }
                else
                {
                    ZCMSPage newPage = new ZCMSPage(pagePublishType);
                    newPage.Status = PageStatus.New;
                    newPage.PageID = Int32.Parse(ViewData["CurrentPageId"].ToString());
                    return View(newPage);
                }
            }
            else
            {
                ZCMSPage newPage = new ZCMSPage(pagePublishType);
                newPage.Status = PageStatus.New;
                ViewData["CurrentPageId"] = new Random().Next();
                newPage.PageID = Int32.Parse(ViewData["CurrentPageId"].ToString());
                return View(newPage);
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PageEditor(ZCMSPage page)
        {
            if (Request.Form["delete-page"] != null && Request.Form["delete-page"] == "true")
            {
                _worker.CmsContentRepository.DeleteCmsPage(page);
                return RedirectToAction("DashBoard");
            }
            TryValidateModel(page);
            ZCMSPage ravenPage = _worker.CmsContentRepository.GetCmsPage(page.PageID.ToString());
            if (ModelState.IsValid)
            {


                if (ravenPage != null)
                {
                    ravenPage.PageID = page.PageID;
                    ravenPage.PageName = page.PageName;
                    
                    for (int i = 0; i < ravenPage.Properties.Count; i++)
                    {
                        if (!(ravenPage.Properties[i] is ImageListProperty))
                        {
                            ravenPage.Properties[i] = page.Properties[i];
                        }
                    }
                                        
                    ravenPage.LastModified = DateTime.Now;
                    ravenPage.LastChangedBy = _worker.AuthenticationRepository.GetCurrentUserName();
                    ravenPage.Status = page.Status;

                    TempData["DocumentSaved"] = CMS_i18n.BackendResources.DocumentSaved;
                    return RedirectToAction("PageEditor", new { pageId = ravenPage.PageID });
                }
                else
                {
                    page.WrittenBy = _worker.AuthenticationRepository.GetCurrentUserName();
                    page.LastChangedBy = page.WrittenBy;
                    page.LastModified = DateTime.Now;
                    page.Created = DateTime.Now;
                    _worker.CmsContentRepository.CreateCmsPage(page, Request.Form["PermissionSet"].ToString());
                }
                return RedirectToAction("PageEditor", new { pageId = page.PageID });
            }
            else
            {
                ViewData["CurrentPageId"] = page.PageID;
                return View(ravenPage == null ? page : ravenPage);
            }
        }

        public ActionResult Dashboard()
        {
            return View("Dashboard");
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

            ZCMSPage pg = _worker.CmsContentRepository.GetCmsPage(actual);
            ZCMSPage pgrev = _worker.CmsContentRepository.GetCmsPage(id.Replace(".", "/"));
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
                        EditUrl = "/"+((ZCMSApplication)HttpContext.ApplicationInstance).MainAdminUrl+"/PageEditor/"+z.PageID
                    });
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(items);
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
