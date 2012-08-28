using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace ZCMS.Core.Business.Content
{
    public class ZCMSPage : ZCMSBasePage
    {        

        public ZCMSPage(params IZCMSProperty[] properties):base(properties)
        {            
        }

        public ZCMSPage()
        {            
            
        }

        public ZCMSPage(dynamic bpt):base((object)bpt)
        {

        }         

        public string GetPropertyValues()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in Properties)
            {
                builder.Append(item.PropertyValue + " ");
            }
            return builder.ToString();
        }

    }
}