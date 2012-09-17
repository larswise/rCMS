using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Auth.Business
{
    public static class PermissionSet
    {
        public static IEnumerable<Permission> GetAvailablePermissions(string permission)
        {
            yield return new Permission() { PermissionDisplay = CMS_i18n.BackendResources.PermissionSetNone, PermissionValue = "None", Selected = (permission == "None") ? true : false };
            yield return new Permission() { PermissionDisplay = CMS_i18n.BackendResources.PermissionSetElevated, PermissionValue = "Elevated", Selected = (permission == "Elevated") ? true : false };
            yield return new Permission() { PermissionDisplay = CMS_i18n.BackendResources.PermissionSetAdmin, PermissionValue = "Admin", Selected = (permission == "Admin") ? true : false };
        }
    }

    public class Permission
    {
        public string PermissionValue { get; set; }
        public string PermissionDisplay { get; set; }
        public bool Selected { get; set; }
    }
}