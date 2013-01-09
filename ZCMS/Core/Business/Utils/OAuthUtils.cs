using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ZCMS.Core.Business.Utils
{
    public class OAuthUtils
    {
        public static string RequestNewTwitterToken(SocialService service)
        {
            string postBody = string.Empty;            

            ServicePointManager.Expect100Continue = false;

            HttpWebRequest hwr =
            (HttpWebRequest)HttpWebRequest.Create(
            @"https://api.twitter.com/oauth/request_token");

            hwr.Method = "POST";
            hwr.Headers.Add("Authorization", GetAuthHeader("https://api.twitter.com/oauth/request_token", "POST", service.Key, service.Secret, string.Empty).Split(';')[0]);
            string sh = hwr.Headers["Authorization"].ToString();
            hwr.ContentType = "application/x-www-form-urlencoded";


            hwr.Timeout = 3 * 60 * 1000;

            try
            {
                WebResponse response = hwr.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                return responseString;
            }
            catch (WebException e)
            {
                return e.Message;
            }
        }

        public static string GetAuthHeader(string url, string method, string key, string secret, string token)
        {
            string hash = string.Empty;
            string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            string oauth_signature_method = "HMAC-SHA1";
            string oauth_timestamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();
            string oauth_version = "1.0";
            
            string baseString = string.Empty;
            string oauthHeader = string.Empty;

            SortedDictionary<string, string> sd = new SortedDictionary<string, string>();
            sd.Add("oauth_version", oauth_version);
            sd.Add("oauth_consumer_key", key);
            sd.Add("oauth_nonce", oauth_nonce);
            sd.Add("oauth_signature_method", oauth_signature_method);
            sd.Add("oauth_timestamp", oauth_timestamp);
            //sd.Add("oauth_callback", oauth_callback);

            StringBuilder preBaseString = new StringBuilder();
            StringBuilder bases = new StringBuilder();
            preBaseString.Append(method + "&");
            preBaseString.Append(Uri.EscapeDataString(
                url));
            preBaseString.Append("&");

            foreach (var keyValuePair in sd)
            {
                if (keyValuePair.Key is string)
                {
                    if (bases.Length > 0)
                    {
                        bases.Append("&");
                    }

                    bases.Append(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}={1}",
                            UrlEncode(keyValuePair.Key),
                            UrlEncode((string)keyValuePair.Value)));
                }
            }
            baseString = preBaseString.ToString() + UrlEncode(bases.ToString());



            var content = string.Concat(Uri.EscapeDataString(secret), "&");
            HMACSHA1 hmac = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(content));

            hash = Convert.ToBase64String(hmac.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            oauthHeader = "OAuth ";
            oauthHeader += "oauth_consumer_key=\"" + UrlEncode(sd["oauth_consumer_key"]) + "\",";
            oauthHeader += "oauth_nonce=\"" + UrlEncode(sd["oauth_nonce"]) + "\",";
            oauthHeader += "oauth_signature_method=\"" + UrlEncode(sd["oauth_signature_method"]) + "\",";
            oauthHeader += "oauth_timestamp=\"" + UrlEncode(sd["oauth_timestamp"]) + "\",";
            if (!String.IsNullOrEmpty(token))
                oauthHeader += "oauth_token=\"" + token + "\",";
            oauthHeader += "oauth_version=\"" + UrlEncode(sd["oauth_version"]) + "\",";
            oauthHeader += "oauth_signature=\"" + UrlEncode(hash) + "\"";

            return oauthHeader + ";oauth_consumer_key=" + UrlEncode(sd["oauth_consumer_key"]) + "&oauth_signature_method=HMAC-SHA1" + (!String.IsNullOrEmpty(token) ? "&oauth_token="+token : "") + "&oauth_timestamp=" + UrlEncode(sd["oauth_timestamp"]) + "&oauth_nonce=" + UrlEncode(sd["oauth_nonce"]) + "&oauth_signature=" + UrlEncode(hash);
        }

        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            value = Uri.EscapeDataString(value);

            // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
            value = Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

            // these characters are not escaped by UrlEncode() but needed to be escaped
            value = value
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("$", "%24")
                .Replace("!", "%21")
                .Replace("*", "%2A")
                .Replace("'", "%27");

            // these characters are escaped by UrlEncode() but will fail if unescaped!
            value = value.Replace("%7E", "~");

            return value;
        }
    }
}