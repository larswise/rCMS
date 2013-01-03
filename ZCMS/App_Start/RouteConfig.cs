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
                name: "ParamApiFilter",
                routeTemplate: "api/{controller}/{action}/{param}",
                defaults: new { param = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "PlainApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // backend...

            routes.MapRoute(
                "FilterFileManager",
                "Backend/FileSelector/{filterFreeText}",
                new { Controller = "File", Action = "FileSelector", filterFreeText = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ApplyImageEffect",
                "File/ApplyImageEffect/{effect}/{imageKey}",
                new { Controller = "File", Action = "ApplyImageEffect", effect = UrlParameter.Optional, imageKey = UrlParameter.Optional }
            );

            string mainAdminPath =
            string.IsNullOrEmpty(((ZCMSApplication)HttpContext.Current.ApplicationInstance).MainAdminUrl)
            ?
            string.Empty : ((ZCMSApplication)HttpContext.Current.ApplicationInstance).MainAdminUrl.TrimEnd('/') + "/";


            string mainFrontendPath =
            string.IsNullOrEmpty(((ZCMSApplication)HttpContext.Current.ApplicationInstance).MainContentUrl)
            ?
            string.Empty : ((ZCMSApplication)HttpContext.Current.ApplicationInstance).MainContentUrl.TrimEnd('/') + "/";
            routes.MapRoute(
                "Frontend",
                mainFrontendPath + "{slug}",
                new { Controller = "Frontend", Action = "ViewPage", slug = UrlParameter.Optional }
            );

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
                "Backend_FileManager",
                mainAdminPath + "FileManager/{mParameter}",
                new { Controller = "Backend", action = "FileManager", mParameter = UrlParameter.Optional }

            );


            routes.MapRoute(
                "Backend_PublishPageWithType",
                mainAdminPath + "{action}/{pageId}/{pageType}",
                new { Controller = "Backend", pageId = UrlParameter.Optional, pageType = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                "Backend_upload",
                mainAdminPath + "UploadAttachment/{pageId}",
                new { Controller = "File", action = "UploadAttachment", pageId = UrlParameter.Optional }
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


            routes.MapRoute(
                "Backend_Generalc",
                mainAdminPath + "{action}/{mParameter}",
                new { Controller = "Backend", mParameter = UrlParameter.Optional }

            );
        }
    }
}