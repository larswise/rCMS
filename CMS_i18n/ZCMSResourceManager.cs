using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CMS_i18n
{
    public static class ZCMSResourceManager
    {
        private static ResourceManager backendRes = new ResourceManager("BackendResources", System.Reflection.Assembly.GetExecutingAssembly());
        private static ResourceManager authRes = new ResourceManager("Auth", System.Reflection.Assembly.GetExecutingAssembly());
        public static string GetByKey(string key, string resource)
        {
            if(resource=="BackendResources")
                return backendRes.GetString(key);
            if(resource=="Auth")
                return authRes.GetString(key);
            return string.Empty;
        }
    }
}
