using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZCMS.Core.Business
{
    public class ZCMSViewEngine : RazorViewEngine
    {
        private static string[] NewViewFormats = new[] { 
                "~/Core/Backend/Views/{0}.cshtml",
                "~/Core/Auth/Views/{0}.cshtml",
                "~/Core/Auth/Views/{0}/{0}.cshtml"
        };

        private static string[] NewPartialViewFormats = new[] { 
                "~/Core/Backend/Views/{1}/Units/{0}.cshtml",
                "~/Core/Backend/Views/Shared/Units/{0}.cshtml",
                "~/Core/Backend/Views/{1}/Partials/{0}.cshtml",
                "~/Core/Backend/Views/Shared/Partials/{0}.cshtml",
                "~/Core/Backend/Views/Shared/Partials/{1}.cshtml",
                "~/Core/Backend/Views/Shared/EditorTemplates/{0}.cshtml",

                "~/Core/Auth/Views/Partials/{0}.cshtml"
        };

        public ZCMSViewEngine()
        {
            base.PartialViewLocationFormats = base.PartialViewLocationFormats.Union(NewPartialViewFormats).ToArray();
            base.ViewLocationFormats = base.ViewLocationFormats.Union(NewViewFormats).ToArray();
        }   
    }
}