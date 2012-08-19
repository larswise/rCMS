
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business.Content
{
    public class ZCMSMetaDataItem
    {
        public string MetaKey { get; set; }
        public string MetaValue { get; set; }
    }

    public class ZCMSMetaData
    {
        public List<ZCMSMetaDataItem> MetaDatas { get; set; }
    }
}