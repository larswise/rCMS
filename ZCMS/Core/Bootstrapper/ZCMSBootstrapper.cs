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
using FluentValidation.Mvc;
using Raven.Client.Document;
using Raven.Client.Extensions;
using ZCMS.Core.Business;
using ZCMS.Core.Business.Content;
using ZCMS.Core.Business.Validators;
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

                worker.CreateIndexes();
                worker.OpenSession();
                var builder = new ContainerBuilder();
                builder.RegisterModelBinderProvider();

                builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
                builder.RegisterControllers(Assembly.GetExecutingAssembly());
                

                builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
                builder.RegisterInstance(worker).SingleInstance();
                builder.RegisterFilterProvider();
                
                if (worker.ConfigRepository.InitialSetup())
                {
                    worker.ConfigRepository.EnsureDbExists(true);
                    worker.AuthenticationRepository.CreateDefaultUserAndRoles();
                    worker.ConfigRepository.WireUpVersioning();
                                        

                    IZCMSPageType pt1 = new ArticlePage();
                    pt1.PageTypeDisplayName = CMS_i18n.BackendResources.PageTypeDisplayArticle;

                    IZCMSPageType pt2 = new ContainerPage();
                    pt2.PageTypeDisplayName = CMS_i18n.BackendResources.PageTypeDisplayContainer;

                    worker.CmsContentRepository.RegisterPageType(pt1);
                    worker.CmsContentRepository.RegisterPageType(pt2);

                    worker.ConfigRepository.SetUpMenus();
                }
                builder.RegisterInstance(worker.CmsContentRepository.GetMainMenus()).SingleInstance();

                ZCMSModelValidatorProvider validatorProvider = new ZCMSModelValidatorProvider();
                ModelValidatorProviders.Providers.Clear();
                ModelValidatorProviders.Providers.Add(validatorProvider);

                builder.RegisterInstance(validatorProvider).SingleInstance();
                var container = builder.Build();

                //FluentValidationModelValidatorProvider.Configure();

                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
                GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

                // check if social media is set to active
                List<SocialService> socialServices = worker.CmsContentRepository.GetSocialServiceConfigs();
                System.Web.HttpContext.Current.Application["ActiveSocialMedias"] = 
                    worker.CmsContentRepository.GetSocialServiceConfigs()
                    .Where(a => a.Activated)
                    .Select(s => s.ServiceName).ToList();
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
            var documentStore = new DocumentStore
            {
                Url = "http://localhost:8088",
                DefaultDatabase = ConfigurationManager.AppSettings["RavenDBDefaultDb"].ToString(),
                Conventions =
                {
                    FindTypeTagName = type => typeof(IZCMSPageType).IsAssignableFrom(type) ? "IZCMSPageType" : null,
                },
                Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["RavenDBWindowsUser"].ToString(), ConfigurationManager.AppSettings["RavenDBWindowsPassword"].ToString()),
            };

            documentStore.Initialize();
            documentStore.JsonRequestFactory.EnableBasicAuthenticationOverUnsecureHttpEvenThoughPasswordsWouldBeSentOverTheWireInClearTextToBeStolenByHackers = true;            
            documentStore.DatabaseCommands.EnsureDatabaseExists(ConfigurationManager.AppSettings["RavenDBDefaultDb"].ToString(), true);
            
            UnitOfWork worker = new UnitOfWork(documentStore);
            return worker;
        }

        
    }
}