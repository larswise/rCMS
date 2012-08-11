using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Raven.Client.Document;
using Raven.Client.Extensions;
using ZCMS.Core.Business;
using ZCMS.Core.Data;

namespace ZCMS.Core.Bootstrapper
{
    public class ZCMSBootstrapper
    {
        public void SetIOCAppContainer()
        {
            UnitOfWork worker = GetUnitOfWork();
            try
            {
                

                worker.OpenSession();
                var builder = new ContainerBuilder();
                builder.RegisterModelBinderProvider();

                builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
                builder.RegisterControllers(Assembly.GetExecutingAssembly());

                builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
                builder.RegisterInstance(worker).SingleInstance();
                builder.RegisterFilterProvider();


                var container = builder.Build();

                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
                GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

                if (worker.ConfigRepository.InitialSetup())
                {
                    worker.ConfigRepository.EnsureDbExists(true);
                    worker.ConfigRepository.WireUpVersioning();
                    worker.AuthenticationRepository.CreateDefaultUserAndRoles();
                    

                    IZCMSPageType pt1 = new ArticlePage();
                    IZCMSPageType pt2 = new ContainerPage();
                    worker.CmsContentRepository.RegisterPageType(pt1);
                    worker.CmsContentRepository.RegisterPageType(pt2);

                    worker.ConfigRepository.SetUpMenus();
                }
            }
            catch (Exception ex)
            {
                // this crashes if config doc exists...
                System.Diagnostics.Debug.Write(ex.Message + "  -  " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                worker.CloseSession();
            }
        }




        private UnitOfWork GetUnitOfWork()
        {
            // ensure dbexists, set default user/pw from config etc...
            var documentStore = new DocumentStore
            {
                Url = "http://localhost:8088",
                DefaultDatabase = ConfigurationManager.AppSettings["RavenDBDefaultDb"].ToString(),                
                Conventions =
                {
                    FindTypeTagName = type => typeof(IZCMSPageType).IsAssignableFrom(type) ? "IZCMSPageType" : null,
                },
                Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["RavenDBDefaultAdminUser"].ToString(), ConfigurationManager.AppSettings["RavenDBDefaultPassword"].ToString()),
            };
            
            documentStore.Initialize();
            documentStore.JsonRequestFactory.EnableBasicAuthenticationOverUnsecureHttpEvenThoughPasswordsWouldBeSentOverTheWireInClearTextToBeStolenByHackers = true;
            
            UnitOfWork worker = new UnitOfWork(documentStore);
            return worker;
        }

        
    }
}