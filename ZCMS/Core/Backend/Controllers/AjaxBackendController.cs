using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using ZCMS.Core.Data;

namespace ZCMS.Core.Backend.Controllers
{
    public class AjaxBackendController : ApiController
    {
        private UnitOfWork _worker;

        public AjaxBackendController(UnitOfWork work)
        {
            _worker = work;
        }



        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        public string UploadAttachment()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(
                    Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data/Uploads");
            var provider = new MultipartFormDataStreamProvider(root);

            // Read the form data and return an async task.
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(readTask =>
                {
                    if (readTask.IsFaulted || readTask.IsCanceled)
                    {
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }

                    // This illustrates how to get the file names.
                    foreach (var file in provider.BodyPartFileNames)
                    {
                        Trace.WriteLine("Client file name: " + file.Key);
                        Trace.WriteLine("Server file path: " + file.Value);
                    }
                    return new HttpResponseMessage(HttpStatusCode.Created);
                });

            return "ljklkj";

             
        }
    }
}
