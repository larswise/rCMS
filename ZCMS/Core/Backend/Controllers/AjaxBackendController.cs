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
    public class AjaxBackendController : ZCMSBaseAjaxController
    {

        public AjaxBackendController(UnitOfWork worker):base(worker)
        {

        }


        public ZCMSPage Get(string id)
        {
            return _worker.CmsContentRepository.GetCmsPage(id);
        }

        public List<ZCMSFileDocument> FileSelector(string filterFreeText)
        {
            return _worker.CmsContentRepository.QueryAttachment(new List<string>(), filterFreeText).Take(25).OrderBy(o => o.Created).Reverse().ToList();
        }
    }
}
