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
using ZCMS.Core.Business;
using ZCMS.Core.Business.Content;
using ZCMS.Core.Data;
using ZCMS.Core.Utils;

namespace ZCMS.Core.Backend.Controllers
{
    public class FileController : ZCMSBaseController
    {
        public FileController(UnitOfWork worker) : base(worker) { }

        public ActionResult FileManagerList(List<string> extensionFilter, string filterFreeText)
        {
            List<ZCMSFileDocument> docs;

            if (extensionFilter == null || extensionFilter.Count == 0)
                docs = _worker.FileRepository.QueryAttachment(new List<string>() { "*" }, string.Empty);
            else
                docs = _worker.FileRepository.QueryAttachment(extensionFilter, filterFreeText);
            return PartialView("FileManagerList", docs.Take(25).OrderBy(o => o.Created).Reverse().ToList());
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

        #region Ajax Helper Methods
        public void GetCurrentImage(string key)
        {
            WebImage wi = new WebImage(_worker.FileRepository.RetrieveAttachment(key));

            wi.Write();
        }

        public ActionResult RenderUnit(string id)
        {
            try
            {
                return PartialView(id);
            }
            catch (Exception e)
            {
                return PartialView("PartialPageError", e.Message);
            }
        }

        public string ApplyImageEffect(string effect, string imageKey)
        {
            var attachment = _worker.FileRepository.RetrieveAttachmentItem(imageKey);
            System.Drawing.Bitmap bitMap = null;
            if (effect == "grayscale")
                bitMap = ImageEffects.Grayscale(new System.Drawing.Bitmap(attachment.Data()));
            else if (effect == "sepia")
                bitMap = ImageEffects.Sepia(new System.Drawing.Bitmap(attachment.Data()), 10);
            else if (effect == "rotate")
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

        public string DeleteAttachments(List<string> files)
        {
            _worker.FileRepository.DeleteAttachments(files);
            return string.Format(CMS_i18n.BackendResources.FileManagerFilesWasDeleted, files.Count);
        }
        #endregion

    }
}
