using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZCMS.Core.Data;

namespace ZCMS.Core.Security
{   
    
    public class ZCMSAuthenticateAttribute : AuthorizeAttribute
    {
        public UnitOfWork worker { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                
                return worker.AuthenticationRepository.AuthorizeCurrentUser();
            }
            catch (NullReferenceException)
            {
                return false;
            }
            
        }
    }

}