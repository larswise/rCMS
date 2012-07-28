using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business
{
    public static class ZCMSPageFactory
    {
        public static dynamic GetPagePublishType(string pageType)
        {
            dynamic pagePublishType;

            try
            {
                pagePublishType = Activator.CreateInstance(Type.GetType("ZCMS.Core.Business." + pageType));
            }
            catch
            {
                pagePublishType = Activator.CreateInstance(typeof(ArticlePage));
            }

            return pagePublishType;            
        }

        public static List<string> GetAllPageTypeNames()
        {
            // todo, add fetching and putting to raven!
            return new List<string>() { "ContainerPage", "ArticlePage" };
        }
    }
}