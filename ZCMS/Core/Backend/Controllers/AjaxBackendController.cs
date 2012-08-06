using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using ZCMS.Core.Business;
using ZCMS.Core.Data;

namespace ZCMS.Core.Backend.Controllers
{
    public class AjaxBackendController : ApiController
    {
        protected UnitOfWork _worker;

        public AjaxBackendController(UnitOfWork worker)
        {
            System.Diagnostics.Debugger.Break();
            _worker = worker;
            _worker.OpenSession();
        }

        public AjaxBackendController()
        {
            System.Diagnostics.Debugger.Break();
        }

        public List<ZCMSFileDocument> FileManagerList(List<string> extensionFilter, string filterFreeText)
        {
            if (extensionFilter == null || extensionFilter.Count == 0)
                return _worker.CmsContentRepository.QueryAttachment(new List<string>() { "*" }, string.Empty);
            else
                return _worker.CmsContentRepository.QueryAttachment(extensionFilter, filterFreeText);
        }

        public ZCMSPage Get()
        {
            return _worker.CmsContentRepository.GetCmsPage("43064");
        }

        public JsonResult GetString(string id)
        {
            return new JsonResult() { Data = "hello", ContentType = "application/json" };
        }

        // GET api/default1
        public IEnumerable<string> StringGet()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
