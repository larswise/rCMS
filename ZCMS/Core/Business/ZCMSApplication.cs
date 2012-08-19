using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Web;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using ZCMS.Core.Data;
using System.Web.Http;
using Raven.Client.Document;
using ZCMS.Core.Bootstrapper;
using System.Web.Security;

namespace ZCMS.Core.Business
{
    public class ZCMSApplication : System.Web.HttpApplication
    {
        protected virtual void Application_Start()
        {
            ZCMSBootstrapper bs = new ZCMSBootstrapper();
                        
            bs.SetIOCAppContainer();

            MainAdminUrl = "Admin";
            MainContentUrl = "Pages";
            
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ZCMSViewEngine());
                        
            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configuration.Formatters.Add(new ZCMSRazorFormatter());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }

        public string MainContentUrl 
        {
            get
            {
                return HttpContext.Current.Application["DefaultFrontendPath"].ToString();
            }
            set
            {
                HttpContext.Current.Application["DefaultFrontendPath"] = value;
            }
        }

        public string MainAdminUrl {
            get
            {
                return HttpContext.Current.Application["DefaultBackendPath"].ToString();
            }
            set
            {
                HttpContext.Current.Application["DefaultBackendPath"] = value;
            }
        }

        static IContainerProvider _containerProvider;

        public IContainerProvider ContainerProvider
        {
            get { return _containerProvider; }
        }
    }
}
