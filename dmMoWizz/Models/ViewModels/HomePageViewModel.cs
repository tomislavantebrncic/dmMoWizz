using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.ViewModels
{
    public class HomePageViewModel
    {
        public List<HomePageMovieViewModel> Watchlist { get; set; }
        public List<HomePageMovieViewModel> Suggested { get; set; }
        public List<HomePageMovieViewModel> Popular { get; set; }

        public HomePageViewModel()
        {
            Watchlist = new List<HomePageMovieViewModel>();
            Suggested = new List<HomePageMovieViewModel>();
            Popular = new List<HomePageMovieViewModel>();
        }
    }

    public class HomePageMovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PosterPath { get; set; }
        public string BackdropPath { get; set; }
        //personalRate je postotak vjerojatnosti da će se korisniku svidjeti film
        public string PersonalRate { get; set; }
        //averageRate je prosječna ocjena filma 1-10
        public string AverageRate { get; set; }
        public string Overview { get; set; }
        public bool AddedToWatchlist { get; set; }
    }
}