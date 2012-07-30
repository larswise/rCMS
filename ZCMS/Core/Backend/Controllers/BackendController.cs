using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Raven.Client;
using ZCMS.Core.Business;
using ZCMS.Core.Data;
using ZCMS.Core.Security;


namespace ZCMS.Core.Backend.Controllers
{
    [ZCMSAuthenticate(Roles = "Administrators")]
    public class BackendController : ZCMSBaseController
    {
        
        public BackendController(UnitOfWork worker):base(worker) { }

        public ActionResult AdminStart()
        {
            return View();
        }

        public ActionResult PageEditor(int ?pageId, string mParameter)
        {
            dynamic pagePublishType = ZCMSPageFactory.GetPagePublishType(mParameter);            
            
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
                            mses.Add(new WebImage(_worker.CmsContentRepository.TransStorageBt(item)));
                        }
                        TempData["ImageCollection"] = mses;
                    }

 
                    return View(page);
                }
                else
                {
                    ZCMSPage newPage = new ZCMSPage(pagePublishType);

                    newPage.PageID = Int32.Parse(ViewData["CurrentPageId"].ToString());
                    return View(newPage);
                }
            }
            else
            {
                ZCMSPage newPage = new ZCMSPage(pagePublishType);
                ViewData["CurrentPageId"] = new Random().Next();
                newPage.PageID = Int32.Parse(ViewData["CurrentPageId"].ToString());
                return View(newPage);
            }

        }

        public void GetCurrentImage(string key)
        {
            WebImage wi = new WebImage(_worker.CmsContentRepository.TransStorageBt(key));
            wi.FileName = "kluss.png";
            wi.Write();
        }

        public ActionResult RenderLeftMenu(string id)
        {
            try
            {                
                var currentMenu = _worker.CmsContentRepository.GetMenu(id);
                return PartialView("SubMenu", TempData["MenuData"]);
            }
            catch(Exception e)
            {
                return View(e);
            }
        }

        public ActionResult Revise(string id)
        {
            string[] pageIdArr = id.Split('.');
            string actual = pageIdArr[0];

            ZCMSPage pg = _worker.CmsContentRepository.GetCmsPage(actual);
            ZCMSPage pgrev = _worker.CmsContentRepository.GetCmsPage(id.Replace(".", "/"));
            pg.Properties = pgrev.Properties;
            pg.StartPublish = pgrev.StartPublish;
            pg.EndPublish = pgrev.EndPublish;
            pg.PageName = pgrev.PageName;
            pg.ShowInMenus = pgrev.ShowInMenus;
            pg.AllowComments = pgrev.AllowComments;

            _worker.CmsContentRepository.SaveCmsPage(pg);

            return RedirectToAction("PageEditor", new { pageId = pg.PageID });
        }

        public ActionResult RenderAllRevisions(string id)
        {
            try
            {
                var items = _worker.CmsContentRepository.GetPastRevisions(id);
                if (items.Count > 0)
                    return PartialView("RenderAllRevisions", _worker.CmsContentRepository.GetPastRevisions(id));
                else
                    return null;
            }
            catch
            {
                return PartialView();
            }
        }

        public ActionResult UploadAttachment(string pageId)
        {
            try
            {
                
                var stream = Request.InputStream;
                MemoryStream mstream = new MemoryStream();
                stream.CopyTo(mstream);
                mstream.Position = 0;
                _worker.CmsContentRepository.StoreAttachment(pageId, Request.QueryString["qqfile"], mstream);
                
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, "application/json");
            }

            return Json(new { success = true }, "text/html");
        }

            //var attachment = _worker.RetrieveAttachments(pageId.ToString());
 

            //Func<Stream> data = attachment.Data;
            //attachment.Data = () =>
            //{
            //    var memoryStream = new MemoryStream();
            //    memoryStream = _worker.TransStorageBt(data, memoryStream);
            //    memoryStream.Position = 0;
            //    return memoryStream;
            //};

            //Stream st = attachment.Data();


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PageEditor(ZCMSPage page)
        {            
            TryValidateModel(page);
            ZCMSPage ravenPage = _worker.CmsContentRepository.GetCmsPage(page.PageID.ToString());
            if (ModelState.IsValid)
            {
                
                
                if (ravenPage != null)
                {
                    ravenPage.EndPublish = page.EndPublish;
                    ravenPage.PageID = page.PageID;
                    ravenPage.PageName = page.PageName;
                    for (int i = 0; i < ravenPage.Properties.Count; i++ )
                    {
                        if (!(ravenPage.Properties[i] is ImageListProperty))
                        {
                            ravenPage.Properties[i] = page.Properties[i];
                        }
                    }
                    
                    ravenPage.StartPublish = page.StartPublish;
                    ravenPage.Status = page.Status;
                    ravenPage.ShowInMenus = page.ShowInMenus;
                    ravenPage.AllowComments = page.AllowComments;

                    _worker.CmsContentRepository.SaveCmsPage(ravenPage);
                    TempData["DocumentSaved"] = CMS_i18n.BackendResources.DocumentSaved;
                    return RedirectToAction("PageEditor", new { pageId = ravenPage.PageID });
                }
                else
                {
                    _worker.CmsContentRepository.CreateCmsPage(page);
                }
                return RedirectToAction("PageEditor", new { pageId = page.PageID });
            }
            else
            {
                ViewData["CurrentPageId"] = page.PageID;
                return View(ravenPage == null ? page: ravenPage);
            }
        }

    }
}
