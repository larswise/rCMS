using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Auth.Business
{
    public class ZCMSUser
    {
        [Required(ErrorMessageResourceType = typeof(CMS_i18n.Auth), ErrorMessageResourceName = "AuthUsernameRequired")]
        public string Username { get; set; }
        [Required(ErrorMessageResourceType = typeof(CMS_i18n.Auth), ErrorMessageResourceName = "AuthPasswordRequired")]
        public string Password { get; set; }
        public string FullName { get; set; }
    }
}