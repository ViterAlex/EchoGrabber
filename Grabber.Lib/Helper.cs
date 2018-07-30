using System.Text.RegularExpressions;
using System.Web;

namespace EchoGrabber
{
    public static class Helper
    {
        public static string Clean(this string text)
        {
            return Regex.Replace(text, @"[\r\n\t]+", "");
        }

        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static string ToSingleString(this string text)
        {
            return text.Replace("\r", "").Replace("\n", "");
        }

        public static bool EchoIsOnline
        {
            get
            {
                var ping = new System.Net.NetworkInformation.Ping();
                try
                {
                    var result = ping.Send("8.8.8.8");
                    return (result.Status == System.Net.NetworkInformation.IPStatus.Success);
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
        }

        public static string DecodeHtml(this string html)
        {
            return HttpUtility.HtmlDecode(html);
        }

    }
}
