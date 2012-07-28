using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using ZCMS.Core.Business;

namespace ZCMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "backend/ajax/UploadAttachment/{pageId}",
                defaults: new { controller = "AjaxBackend", action = "UploadAttachment", pageId = RouteParameter.Optional }
            );
            // backend...

            string mainAdminPath =
            string.IsNullOrEmpty(((ZCMSApplication)HttpContext.Current.ApplicationInstance).MainAdminUrl)
            ?
            string.Empty : ((ZCMSApplication)HttpContext.Current.ApplicationInstance).MainAdminUrl.TrimEnd('/') + "/";

            routes.MapRoute(
                "Backend_PublishPageTypeNoID",
                mainAdminPath + "PageEditor/{pageType}",
                new { Controller = "Backend", action = "PageEditor", pageType = UrlParameter.Optional },
                new
                {
                    pageType = @"^[A-z]+$"
                }
            );

            routes.MapRoute(
                "Backend_PublishPageWithType",
                mainAdminPath + "PageEditor/{pageId}/{pageType}",
                new { Controller = "Backend", action = "PageEditor", pageId = UrlParameter.Optional, pageType = UrlParameter.Optional }
            );



            routes.MapRoute(
                "Backend_upload",
                mainAdminPath + "UploadAttachment/{pageId}",
                new { Controller = "Backend", action = "UploadAttachment", pageId = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Auth_logon",
                "auth/{action}",
                new { Controller = "Authentication", action = "SignIn" }
            );


            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}