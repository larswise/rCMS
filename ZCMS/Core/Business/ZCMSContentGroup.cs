using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business
{
    public class ZCMSContentGroup
    {
        public string GroupName { get; set; }
        public bool DisplayInMenu { get; set; }
        public int NumItems { get; set; }
    }
}