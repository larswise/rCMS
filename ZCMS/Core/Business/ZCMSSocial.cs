using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZCMS.Core.Business
{
    public class ZCMSSocial
    {
        public SocialService Facebook { get; set; }
        public SocialService Twitter { get; set; }
        public SocialService Disqus { get; set; }
        public ZCMSSocial(List<SocialService> services)
        {
            Facebook = services.FirstOrDefault(s => s.ServiceName == "Facebook") as SocialService;
            Twitter = services.FirstOrDefault(s => s.ServiceName == "Twitter") as SocialService;
            Disqus = services.FirstOrDefault(s => s.ServiceName == "Disqus") as SocialService;
        }

        public ZCMSSocial() { }

    }

    public class SocialService
    {        
        public SocialService() { }

        public string JsIntegration { get; set; }
        public string ServiceName { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string PublicToken { get; set; }
        public string PrivateToken { get; set; }
        public bool Activated { get; set; }

    }

    
}
