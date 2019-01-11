using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models
{
    public class IPStackResponse
    {
        public string ip { get; set; }
        public string type { get; set; }
        public string continent_code { get; set; }
        public string continent_name { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string region_code { get; set; }
        public string region_name { get; set; }
        public string city { get; set; }
        public string zip { get; set; }

        public float latitude { get; set; }
        public float longitude { get; set; }

        public IPStackResponseLocation location { get; set; }

    }

    public class IPStackResponseLocation
    {
        public int geoname_id { get; set; }
        public string capital { get; set; }

        IList<IPStackResponseLanguage> languages { get; set; }

        public string country_flag { get; set; }
        public string country_flag_emoji { get; set; }
        public string country_flag_emoji_unicode { get; set; }
        public string calling_code { get; set; }
        public bool is_eu { get; set; }
    }

    public class IPStackResponseLanguage
    {
        public string code { get; set; }
        public string name { get; set; }
        public string native { get; set; }
    }
}