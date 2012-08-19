using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business
{
    public class ZCMSDashBoard
    {
        public List<ZCMSPage> PublishedPages { get; set; }
        public List<ZCMSPage> DraftedPages { get; set; }

        public List<ZCMSFileDocument> Files { get; set; }
    }
}