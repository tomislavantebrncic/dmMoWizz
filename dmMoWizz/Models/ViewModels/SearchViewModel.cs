using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.ViewModels
{
    public class SearchViewModel
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public List<GenreViewModel> GenresDropdown { get; set; }
        public int[] Genre { get; set; }
        //sort can be either name, rate, year
        public string Sort { get; set; }

        public SearchViewModel()
        {
            GenresDropdown = new List<GenreViewModel>();
        }
    }

}