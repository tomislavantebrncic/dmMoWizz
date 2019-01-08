using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.TMDB
{
    public class TMDBPopularMoviesResponse
    {
        public int page { get; set; }
        public int total_results { get; set; }
        public int total_pages { get; set; }
        public PopularMovie[] results { get; set; }

    }

    public class PopularMovie
    {
        public string vote_count { get; set; }
        public int id { get; set; }
        public string video { get; set; }
        public string title { get; set; }
        public string popularity { get; set; }
        public string poster_path { get; set; }
        public string original_language { get; set; }
        public int[] genre_ids { get; set; }
        public string backdrop_path { get; set; }
        public string overview { get; set; }
        public string release_date { get; set; }
    }
}