using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.ViewModels
{
    public class PopularMovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string PosterURL { get; set; }
        //1-10
        public string AverageVote { get; set; }
        //in percents, include % in string
        public string PersonalRate { get; set; }
        //short description
        public string Overview { get; set; }
        //fill with a few 1-5 most important cast members
        public List<CastPersonViewModel> Cast { get; set; }
        public bool AddedToWatchlist { get; set; }
        public List<GenreViewModel> Genres { get; set; }
    }
}