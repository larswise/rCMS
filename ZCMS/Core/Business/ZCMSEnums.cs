using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business
{
    public enum PageStatus
    {
        [Display(Name = "Published")]
        Published,
        [Display(Name = "Draft")]
        Draft,
        [Display(Name = "New")]
        New
    }
}