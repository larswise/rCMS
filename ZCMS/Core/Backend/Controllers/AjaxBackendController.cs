using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using ZCMS.Core.Business;
using ZCMS.Core.Data;

namespace ZCMS.Core.Backend.Controllers
{
    public class AjaxBackendController : ZCMSBaseAjaxController
    {
        public AjaxBackendController(UnitOfWork worker):base(worker)
        {

        }


        public ZCMSPage GetPage(string id)
        {
            return _worker.CmsContentRepository.GetCmsPage(id);
        }

        [System.Web.Http.HttpGet]
        public List<ZCMSFileDocument> FileSelector(string filter)
        {
            return _worker.FileRepository.QueryAttachment(new List<string>() { "*" }, filter).Take(25).OrderBy(o => o.Created).Reverse().ToList();
        }

        [System.Web.Http.HttpPost]
        public List<ZCMSFileDocument> AttachFilesToPage(SimpleParameter param)
        {
            return _worker.FileRepository.AttachToPage(param.Keys, param.Id);
        }

        [System.Web.Http.HttpPost]
        public string RemoveAttachmentFromPage(MultiSimpleParameter param)
        {
            try
            {
                return _worker.FileRepository.DetachFromPage(param.Param1.Split('=')[1], param.Param2);
            }
            catch
            {
                return "failed to detach...";
            }
        }

        [System.Web.Http.HttpGet]
        public dynamic GetPages(string filter)
        {
            var items = _worker.CmsContentRepository.SearchPages(filter).Select(z =>
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
            return items;
        }
        
    }

    public class SimpleParameter
    {
        public List<string> Keys { get; set; }
        public string Id { get; set; }
    }

    public class MultiSimpleParameter
    {
        public string Param1 { get; set; }
        public string Param2 { get; set; }
    }
}
