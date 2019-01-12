using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.ViewModels
{
    public class SearchResultViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string PosterURL { get; set; }
        public string AverageVote { get; set; }
        //short description
        public string Overview { get; set; }
        public List<GenreViewModel> Genres { get; set; }
        //fill with a few 1-5 most important cast members
        public List<CastPersonViewModel> Cast {get; set; }

        public SearchResultViewModel()
        {
            Cast = new List<CastPersonViewModel>();
        }
    }
}