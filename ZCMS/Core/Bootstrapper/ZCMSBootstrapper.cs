using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Raven.Client.Document;
using ZCMS.Core.Business;
using ZCMS.Core.Data;

namespace ZCMS.Core.Bootstrapper
{
    public static class ZCMSBootstrapper
    {
        public static void SetIOCAppContainer(UnitOfWork worker)
        {

            try
            {
                var builder = new ContainerBuilder();
                builder.RegisterModelBinderProvider();                

                builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
                builder.RegisterControllers(Assembly.GetExecutingAssembly());
                
                builder.RegisterInstance(worker).SingleInstance();
                builder.RegisterFilterProvider();

                builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

                var container = builder.Build();
           
                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
                GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

                if (worker.ConfigRepository.InitialSetup())
                {

                    worker.ConfigRepository.WrapUpVersioning();
                    IZCMSPageType pt1 = new ArticlePage();
                    IZCMSPageType pt2 = new ContainerPage();
                    worker.CmsContentRepository.RegisterPageType(pt1);
                    worker.CmsContentRepository.RegisterPageType(pt2);
                    worker.AuthenticationRepository.CreateDefaultUserAndRoles();
                    
                    worker.ConfigRepository.SetUpMenus();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "  -  " + ex.StackTrace);
            }
        }




        public static UnitOfWork GetUnitOfWork()
        {
            var documentStore = new DocumentStore
            {
                Url = "http://localhost:8088",

                Conventions =
                {
                    FindTypeTagName = type => typeof(IZCMSPageType).IsAssignableFrom(type) ? "IZCMSPageType" : null,
                }
            };

            documentStore.Initialize();
            UnitOfWork worker = new UnitOfWork(documentStore, documentStore.OpenSession());
            return worker;
        }

        
    }
}