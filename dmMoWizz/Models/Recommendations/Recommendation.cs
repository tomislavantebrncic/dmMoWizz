using dmMoWizz.Models.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.Recommendations
{
    public class Recommendation : IComparable<Recommendation>
    {
        public MovieInfo Movie { get; set; }
        public double Rating { get; set; }

        public void Update(double incValue)
        {
            Rating += incValue;
        }

        public int CompareTo(Recommendation other)
        {
            return other.Rating.CompareTo(Rating);
        }

        public override bool Equals(object obj)
        {
            var recommendation = obj as Recommendation;
            return recommendation != null &&
                   EqualityComparer<MovieInfo>.Default.Equals(Movie, recommendation.Movie);
        }

        public override int GetHashCode()
        {
            return -1729776441 + EqualityComparer<MovieInfo>.Default.GetHashCode(Movie);
        }
    }
}