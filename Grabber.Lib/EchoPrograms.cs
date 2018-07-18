using System;
using System.Collections.Generic;

namespace EchoGrabber
{
    public static class EchoPrograms
    {
        public static List<PodcastInfo> Actual { get; set; }
        public static List<PodcastInfo> Archived { get; set; }
        private const string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        public static readonly Dictionary<int, Predicate<object>> Filters = new Dictionary<int, Predicate<object>>
        {
            {0, (podInfo) => true},
            {1, (podInfo) => {
                var s = (podInfo as PodcastInfo).Title;
                return char.IsNumber(s[0]);
                }
            },
            {2, (podInfo) => {
                var s = (podInfo as PodcastInfo).Title;
                return Array.IndexOf(alphabet.Substring(0, 4).ToCharArray(), s.ToLower()[0]) != -1 ;
                }
            },
            {3, (podInfo) => {
                var s = (podInfo as PodcastInfo).Title;
                return Array.IndexOf(alphabet.Substring(4, 5).ToCharArray(), s.ToLower()[0]) != -1;
                }
            },
            {4, (podInfo) => {
                var s = (podInfo as PodcastInfo).Title;
                return Array.IndexOf(alphabet.Substring(9, 5).ToCharArray(), s.ToLower()[0]) != -1;
                }
            },
            {5, (podInfo) => {
                var s = (podInfo as PodcastInfo).Title;
                return Array.IndexOf(alphabet.Substring(14, 4).ToCharArray(), s.ToLower()[0]) != -1;
                }
            },
            {6, (podInfo) => {
                var s = (podInfo as PodcastInfo).Title;
                return Array.IndexOf(alphabet.Substring(18, 4).ToCharArray(), s.ToLower()[0]) != -1;
                }
            },
            {7, (podInfo) => {
                var s = (podInfo as PodcastInfo).Title;
                return Array.IndexOf(alphabet.Substring(22, 4).ToCharArray(), s.ToLower()[0]) != -1;
                }
            },
            {8, (podInfo) => {
                var s = (podInfo as PodcastInfo).Title;
                return Array.IndexOf(alphabet.Substring(26, 7).ToCharArray(), s.ToLower()[0]) != -1;
                }
            }
        };
    }
}
