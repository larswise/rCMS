using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client.Document;
using ZCMS.Core.Business;
using ZCMS.Core.Data;

namespace ZCMSUnitTest
{
    [TestClass]
    public class TestController1 : Controller
    {
        UnitOfWork worker;
        public TestController1()
        {
            // Init ravendb embedded mode...
            var documentStore = new DocumentStore
            {
                
                Conventions =
                {
                    FindTypeTagName = type => typeof(IZCMSPageType).IsAssignableFrom(type) ? "IZCMSPageType" : null,
                }
            };
            documentStore.Initialize();
            
            worker = new UnitOfWork(documentStore);

        }

        [TestMethod]
        public void TestMethod1()
        {
            worker.ConfigRepository.WrapUpVersioning();

            ZCMSPage page = new ZCMSPage(new ArticlePage());
            worker.CmsContentRepository.SaveCmsPage(page);
        }
    }
}
