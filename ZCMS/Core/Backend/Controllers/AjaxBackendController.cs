using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
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
            return _worker.CmsContentRepository.QueryAttachment(new List<string>() {"*"}, filter).Take(25).OrderBy(o => o.Created).Reverse().ToList();
        }

        [System.Web.Http.HttpPost]
        public List<ZCMSFileDocument> AttachFilesToPage(SimpleParameter param)
        {
            return _worker.CmsContentRepository.AttachToPage(param.Keys, param.Id);
        }

        [System.Web.Http.HttpPost]
        public string RemoveAttachmentFromPage(MultiSimpleParameter param)
        {
            try
            {
                return _worker.CmsContentRepository.DetachFromPage(param.Param1.Split('=')[1], param.Param2);
            }
            catch
            {
                return "failed to detach...";
            }
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
