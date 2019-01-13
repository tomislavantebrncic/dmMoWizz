using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.ViewModels
{
    public class SummaryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string Backdrop { get; set; }
        public CastPersonViewModel[] Cast {get; set; }
        public string AverageVote { get; set; }
        //percentage that user will like it
        public string PersonalRate { get; set; }
        public string Year { get; set; }
        public bool AddedToWatchlist { get; set; }
        public string Trailer { get; set; }
    }
}