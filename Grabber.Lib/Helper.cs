using System.Text.RegularExpressions;

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
            return text.Replace("\r","").Replace("\n", "");
        }

    }
}
