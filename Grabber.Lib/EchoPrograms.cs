using System;
using System.Collections.Generic;

namespace EchoGrabber
{
    public static class EchoPrograms
    {
        public static List<PodcastInfo> Actual { get; set; }
        public static List<PodcastInfo> Archived { get; set; }
        private const string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        //Предикаты для фильтра по алфавиту
        public static readonly Dictionary<int, Predicate<object>> Filters = new Dictionary<int, Predicate<object>>
        {
            {0, (podInfo) => true},//Все
            {1, (podInfo) => 
                {//1-9
                  return char.IsNumber((podInfo as PodcastInfo).Title[0]);
                }
            },
            {2, (podInfo) => 
                {//А-Г
                    var title = (podInfo as PodcastInfo).Title.ToLower();
                    return Array.IndexOf(alphabet.Substring(0, 4).ToCharArray(), title[0]) != -1 ;
                }
            },
            {3, (podInfo) => 
                {//Д-З
                    var title = (podInfo as PodcastInfo).Title.ToLower();
                    return Array.IndexOf(alphabet.Substring(4, 5).ToCharArray(), title[0]) != -1;
                }
            },
            {4, (podInfo) => 
                {//И-М
                    var title = (podInfo as PodcastInfo).Title.ToLower();
                    return Array.IndexOf(alphabet.Substring(9, 5).ToCharArray(), title[0]) != -1;
                }
            },
            {5, (podInfo) => 
                {//Н-Р
                    var title = (podInfo as PodcastInfo).Title.ToLower();
                    return Array.IndexOf(alphabet.Substring(14, 4).ToCharArray(), title[0]) != -1;
                }
            },
            {6, (podInfo) => 
                {//С-Ф
                    var title = (podInfo as PodcastInfo).Title.ToLower();
                    return Array.IndexOf(alphabet.Substring(18, 4).ToCharArray(), title[0]) != -1;
                }
            },
            {7, (podInfo) => 
                {//Х-Ш
                    var title = (podInfo as PodcastInfo).Title.ToLower();
                    return Array.IndexOf(alphabet.Substring(22, 4).ToCharArray(), title[0]) != -1;
                }
            },
            {8, (podInfo) => 
                {//Щ-Я
                    var title = (podInfo as PodcastInfo).Title.ToLower();
                    return Array.IndexOf(alphabet.Substring(26, 7).ToCharArray(), title[0]) != -1;
                }
            }
        };
    }
}
