using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace EchoGrabber
{
    public static class Helper
    {
        /// <summary>
        /// Парсер даты из строкового представления на сайте
        /// </summary>
        public static DateTime ParseDateTime(this string text)
        {
            var dict = new Dictionary<string, int>()
            {
                {"января", 1},
                {"февраля", 2},
                {"марта", 3},
                {"апреля", 4},
                {"мая", 5},
                {"июня", 6},
                {"июля", 7},
                {"августа", 8},
                {"сентября", 9},
                {"октября", 10},
                {"ноября", 11},
                {"декабря", 12}
            };
            var vals = text.Split(' ');
            vals[1] = dict[vals[1].ToLower()].ToString();
            var properString = string.Join(" ", vals);
            var cult = CultureInfo.CreateSpecificCulture("ru-ru");
            return DateTime.ParseExact(properString, "dd M yyyy, HH:mm", cult);
        }
        /// <summary>
        /// Парсер длительности выпуска из строкового представления на сайте
        /// </summary>
        public static TimeSpan ParseDuration(this string text)
        {
            var format = text.Length == 5 ? @"mm\:ss" : @"hh\:mm\:ss";
            return TimeSpan.ParseExact(text, format, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Парсер размера файла из строкового преставления на сайте
        /// </summary>
        public static float ParseSize(this string text)
        {
            var val = text.Trim().Split(' ')[0];
            return float.Parse(val, CultureInfo.InvariantCulture);
        }

        public static string Clean(this string text)
        {
            return Regex.Replace(text, @"[\r\n\t]+", "").DecodeHtml();
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
