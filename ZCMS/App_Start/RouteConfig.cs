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
                name: "ApiRouteForAttach",
                routeTemplate: "api/{controller}/AttachFilesToPage",
                defaults: new { Controller = "AjaxBackend", Action = "AttachFilesToPage" }
            );

            routes.MapHttpRoute(
                name: "ApiRouteForDetach",
                routeTemplate: "api/{controller}/DetachFilesFromPage",
                defaults: new { Controller = "AjaxBackend", Action = "DetachFilesFromPage" }
            );


            routes.MapHttpRoute(
                name: "DefaultApiFilter",
                routeTemplate: "api/{controller}/{action}/{filter}",
                defaults: new { filter = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            

            // backend...

            routes.MapRoute(
                "FilterFileManager",
                "Backend/FileSelector/{filterFreeText}",
                new { Controller = "Backend", Action = "FileSelector", filterFreeText = UrlParameter.Optional }
            );


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