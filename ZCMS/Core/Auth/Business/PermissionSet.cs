using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Auth.Business
{
    public static class PermissionSet
    {
        public static IEnumerable<Permission> GetAvailablePermissions()
        {
            yield return new Permission() { PermissionDisplay = CMS_i18n.BackendResources.PermissionSetNone, PermissionValue = "None" };
            yield return new Permission() { PermissionDisplay = CMS_i18n.BackendResources.PermissionSetElevated, PermissionValue = "Elevated" };
            yield return new Permission() { PermissionDisplay = CMS_i18n.BackendResources.PermissionSetAdmin, PermissionValue = "Admin" };
        }
    }

    public class Permission
    {
        public string PermissionValue { get; set; }
        public string PermissionDisplay { get; set; }
    }
}