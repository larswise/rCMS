using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business.Content
{
    public class ZCMSTopics
    {
        public ZCMSTopics()
        {
            Topics = new List<ZCMSTopic>();           
        }

        public List<ZCMSTopic> Topics { get; set; }
        public List<string> AvailableColors { get; set; }
    }
        
    public class ZCMSTopic
    {
        public ZCMSTopic()
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool ShowInMenu { get; set; }
        public string Color { get; set; }
        public int? TopicId { get; set; }
    }
}