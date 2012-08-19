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
using ZCMS.Core.Business;
using ZCMS.Core.Data;
using ZCMS.Core.Security;
using ZCMS.Core.Utils;


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

        #region Page Actions
        public ActionResult PageEditor(int ?pageId, string mParameter)
        {
            dynamic pagePublishType = ZCMSPageFactory.GetPagePublishType(mParameter);        
    
            // just testing...
            var items = _worker.FileRepository.GetAllFileTypes();
            var recent = _worker.FileRepository.GetN_MostRecentAttachments(5);

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
                    ravenPage.EndPublish = page.EndPublish;
                    ravenPage.PageID = page.PageID;
                    ravenPage.PageName = page.PageName;
                    ravenPage.UrlSlug = page.UrlSlug;
                    for (int i = 0; i < ravenPage.Properties.Count; i++)
                    {
                        if (!(ravenPage.Properties[i] is ImageListProperty))
                        {
                            ravenPage.Properties[i] = page.Properties[i];
                        }
                    }

                    ravenPage.StartPublish = page.StartPublish;
                    ravenPage.LastModified = DateTime.Now;
                    ravenPage.LastChangedBy = _worker.AuthenticationRepository.GetCurrentUserName();
                    ravenPage.Status = page.Status;
                    ravenPage.ShowInMenus = page.ShowInMenus;
                    ravenPage.AllowComments = page.AllowComments;

                    _worker.CmsContentRepository.SaveCmsPage(ravenPage);
                    TempData["DocumentSaved"] = CMS_i18n.BackendResources.DocumentSaved;
                    return RedirectToAction("PageEditor", new { pageId = ravenPage.PageID });
                }
                else
                {
                    page.WrittenBy = _worker.AuthenticationRepository.GetCurrentUserName();
                    page.LastChangedBy = page.WrittenBy;
                    page.LastModified = DateTime.Now;
                    page.Created = DateTime.Now;
                    _worker.CmsContentRepository.CreateCmsPage(page);
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
            pg.StartPublish = pgrev.StartPublish;
            pg.EndPublish = pgrev.EndPublish;
            pg.PageName = pgrev.PageName;
            pg.ShowInMenus = pgrev.ShowInMenus;
            pg.AllowComments = pgrev.AllowComments;

            _worker.CmsContentRepository.SaveCmsPage(pg);

            return RedirectToAction("PageEditor", new { pageId = pg.PageID });
        }
        #endregion

        #region Ajax Helper Methods
        public void GetCurrentImage(string key)
        {
            WebImage wi = new WebImage(_worker.FileRepository.RetrieveAttachment(key));
            
            wi.Write();
        }

        public string ApplyImageEffect(string effect, string imageKey)
        {
            var attachment = _worker.FileRepository.RetrieveAttachmentItem(imageKey);
            System.Drawing.Bitmap bitMap = null;
            if (effect == "grayscale")
                bitMap = ImageEffects.Grayscale(new System.Drawing.Bitmap(attachment.Data()));
            else if (effect == "sepia")
                bitMap = ImageEffects.Sepia(new System.Drawing.Bitmap(attachment.Data()), 10);
            else if(effect == "rotate")
                bitMap = ImageEffects.Rotate(new System.Drawing.Bitmap(attachment.Data()), 90);
            
            if (bitMap != null)
            {
                
                MemoryStream imageMs = new MemoryStream();
                bitMap.Save(imageMs, System.Drawing.Imaging.ImageFormat.Jpeg);
                imageMs.Position = 0;
                _worker.FileRepository.UpdateAttachment(imageKey, imageMs);
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(imageKey);
        }

        public string GetImages(List<string> key)
        {
            var items = _worker.FileRepository.RetrieveMultipleAttachments(key);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(items);
        }

        public string GetPages(string key)
        {
            var items = _worker.CmsContentRepository.GetRecentPages()
                .Select(z => 
                    new 
                    { 
                        PageName = z.PageName, 
                        PageId = z.PageID,  
                        Created = z.Created.ToString(CMS_i18n.Formats.DateFormat),
                        LastModified = z.LastModified.ToString(CMS_i18n.Formats.DateFormat),
                        CreatedBy = z.WrittenBy,
                        LastModifiedBy = z.LastChangedBy,
                        Status = z.Status,
                        StartPublish = z.StartPublish.Value.ToString(CMS_i18n.Formats.DateFormat),
                        EndPublish = z.EndPublish.HasValue ? z.EndPublish.Value.ToString(CMS_i18n.Formats.DateFormat) : string.Empty
                    });
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(items);
        }

        public string DeleteAttachments(List<string> files)
        {
            _worker.FileRepository.DeleteAttachments(files);
            return string.Format(CMS_i18n.BackendResources.FileManagerFilesWasDeleted, files.Count);
        }
        #endregion

        #region Partial view rendering
        public ActionResult RenderLeftMenu(string id)
        {
            try
            {
                HttpCookie cookie = new HttpCookie("active-menu");

                cookie.Value = id;
                Response.Cookies.Add(cookie);

                return PartialView("SubMenu", _worker.CmsContentRepository.GetMenu(id));
            }
            catch(Exception e)
            {
                return View(e);
            }
        }


        public ActionResult FileManagerList(List<string> extensionFilter, string filterFreeText)
        {
            List<ZCMSFileDocument> docs;

            if (extensionFilter == null || extensionFilter.Count == 0)
                docs = _worker.FileRepository.QueryAttachment(new List<string>() { "*" }, string.Empty);
            else
                docs = _worker.FileRepository.QueryAttachment(extensionFilter, filterFreeText);
            return PartialView("FileManagerList", docs.Take(25).OrderBy(o => o.Created).Reverse().ToList());
        }



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

        public ActionResult FileSelector(string filterFreeText)
        {
            return PartialView("FileSelector", _worker.FileRepository.QueryAttachment(
                new List<string>(System.Configuration.ConfigurationManager.AppSettings["ImageFileFormats"].ToString().Split(',')), filterFreeText).Take(25).OrderBy(o => o.Created).Reverse().ToList());
        }

        public ActionResult FileEditor(string id)
        {
            bool selected = false;
            var file = _worker.FileRepository.Get(id);
            List<SelectListItem> items = new List<SelectListItem>();
            NameValueCollection contentTypes = (NameValueCollection)ConfigurationManager.GetSection("FileContentTypes");
            foreach (var key in contentTypes.AllKeys)
            {
                selected = (key.Equals(file.Extension.ToLower()));
                SelectListItem itm = new SelectListItem() { Text = contentTypes[key].ToString(), Value = key, Selected = selected };
                items.Add(itm);
            }
            ViewData["AvailableContentTypes"] = items;

            return PartialView("EditFile", _worker.FileRepository.Get(id));
        }

        public ActionResult RenderUnit(string id)
        {
            try
            {                
                return PartialView(id);
            }
            catch(Exception e)
            {
                return PartialView("PartialPageError", e.Message);
            }
        }

        public ActionResult UploadAttachment()
        {
            try
            {
                HttpPostedFileBase postedFile = null;
                Stream postedFileStream = null;

                string fileName = string.Empty;
                // Internet Exploder!
                if (Request.Files["qqfile"] != null)
                {
                    postedFile = Request.Files["qqfile"] as HttpPostedFileBase;
                    postedFileStream = postedFile.InputStream;
                    fileName = postedFile.FileName;
                }
                // firefox / chrome etc
                else if (Request.InputStream != null)
                {
                    postedFileStream = Request.InputStream;
                    fileName = Request.QueryString["qqfile"] as string;
                }

                MemoryStream mstream = new MemoryStream();
                postedFileStream.CopyTo(mstream);
                mstream.Position = 0;

                ZCMSFileDocument fDocument = new ZCMSFileDocument(fileName, CMS_i18n.BackendResources.FileManagerUploadedDescription);

                _worker.FileRepository.StoreAttachment(fDocument, mstream);
                
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, "text/html");
            }

            return Json(new { success = true }, "text/html");
        }

        #endregion

    }
}
