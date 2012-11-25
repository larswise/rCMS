using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;

namespace ZCMS.Core.Business
{
    public class ZCMSGlobalConfig
    {
        List<SocialService> _socialServices;

        public List<SocialService> SocialServices
        {
            get
            {
                return _socialServices;
            }
            set
            {
                _socialServices = value;
            }
        }

        public static ZCMSGlobalConfig Instance 
        {
            get
            {
                ZCMSGlobalConfig _instance;
                if ((_instance = Autofac.Integration.Mvc.AutofacDependencyResolver.Current.ApplicationContainer.Resolve<ZCMSGlobalConfig>()) != null)
                    return _instance;
                else
                    return new ZCMSGlobalConfig();
            }
        }
    }
}