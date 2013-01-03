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
using ZCMS.Core.Business.Content;
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
            return _worker.CmsContentRepository.GetCmsPage(id).Instance;
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

        [System.Web.Http.HttpPost]
        public dynamic PostGetPages(MultiSimpleParameter filter)
        {
            PageStatus status = !String.IsNullOrEmpty(filter.Param2) ? (PageStatus)Enum.Parse(typeof(PageStatus), filter.Param2) : PageStatus.Any;
            var items = _worker.CmsContentRepository.SearchPages(filter.Param1, status);
            var items2 = items.Select(z =>
                    new
                    {
                        PageName = z.PageName,
                        PageId = z.PageID,
                        Created = z.Created.ToString(CMS_i18n.Formats.DateFormat),
                        LastModified = z.LastModified.ToString(CMS_i18n.Formats.DateFormat),
                        CreatedBy = z.WrittenBy,
                        LastModifiedBy = z.LastChangedBy,
                        Status = z.Status.ToString(),
                        StartPublish = z.StartPublish,//Properties.Where(p => p.PropertyName == "Start publish").FirstOrDefault().PropertyValue,
                        EndPublish = z.EndPublish, //Properties.Where(p => p.PropertyName == "End Publish").FirstOrDefault().PropertyValue,
                        PageType = z.PageType,
                        EditUrl = "/" + ((ZCMSApplication)HttpContext.Current.ApplicationInstance).MainAdminUrl + "/PageEditor/" + z.PageID,
                        ViewUrl = "/" + ((ZCMSApplication)HttpContext.Current.ApplicationInstance).MainContentUrl +"/"+ z.SlugValue
                    });
            return items2;
        }

        [System.Web.Http.HttpPost]
        public dynamic PostTopic(ZCMSTopic mtopic)
        {
            try
            {
                var allTopics = _worker.CmsContentRepository.GetTopics();

                if (!mtopic.TopicId.HasValue || mtopic.TopicId.Value==0)
                {
                    mtopic.TopicId = new Random().Next();
                    allTopics.Topics.Add(mtopic);
                }
                else
                {
                    var editTopic = allTopics.Topics.Where(t => t.TopicId == mtopic.TopicId).FirstOrDefault();
                    if (editTopic != null)
                    {
                        editTopic.Color = mtopic.Color;
                        editTopic.Description = mtopic.Description;
                        editTopic.Name = mtopic.Name;
                        editTopic.ShowInMenu = mtopic.ShowInMenu;
                    }
                }
                return true;
            }
            catch
            {
                return false;
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
