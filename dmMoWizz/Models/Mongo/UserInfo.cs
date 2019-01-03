using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.Mongo
{
    public class UserInfo
    {
        public string Id { get; set; }
        public string Gender { get; set; }
        public HashSet<int> Watchlist { get; set; }
        public HashSet<int> Watched { get; set; }
        public HashSet<MovieRating> Ratings { get; set; }

        public bool AddToWatchlist(int id)
        {
            return Watchlist.Add(id);
        }

        public void UpdateRatings(MovieRating rating)
        {
            if (!Ratings.Add(rating))
            {
                Ratings.First(r => r.Equals(rating)).Rating = rating.Rating;
            }
        }

        public bool IsRatedMovie(int movieId)
        {
            return Ratings.FirstOrDefault(r => r.MovieId == movieId) != null;
        }
    }

    public class MovieRating
    {
        public int MovieId { get; set; }
        public int Rating { get; set; }

        public override bool Equals(object obj)
        {
            var rating = obj as MovieRating;
            return rating != null &&
                   MovieId == rating.MovieId;
        }

        public override int GetHashCode()
        {
            return 1151147474 + MovieId.GetHashCode();
        }
    }
}