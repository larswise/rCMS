using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ZCMS.Core.Business.Utils
{
    public static class StringUtils
    {
        public static string EncodeToUpper(string raw)
        {            
            return Regex.Replace(raw, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
        }
    }
}