using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.ViewModels
{
    public class MovieDetailsViewModel
    {
        public string BackdropPath { get; set; }
        public string Budget { get; set; }
        public GenreViewModel[] Genres { get; set; }
        public string Homepage { get; set; }
        public int Id { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public string Popularity { get; set; }
        public string PosterPath { get; set; }
        public CompanyViewModel[] ProductionCompanies { get; set; }
        public CountryViewModel[] ProductionCountries { get; set; }
        public string ReleaseDate { get; set; }
        public string Revenue { get; set; }
        public CountryViewModel[] SpokenLanguages { get; set; }
        public string Status { get; set; }
        public string Tagline { get; set; }
        public string Title { get; set; }
        public string VoteAverage { get; set; }
        public int VoteCount { get; set; }

        public CreditsViewModel Credits { get; set; }
        public SimilarMovieViewModel[] SimilarMovies { get; set; }

        public ImageViewModel[] Posters { get; set; }
        public ImageViewModel[] Backdrops { get; set; }
        public TrailerViewModel[] Trailers { get; set; }
        public RatingViewModel[] Ratings { get; set; }
    }

    public class CreditsViewModel
    {
        public CastPersonViewModel[] Cast { get; set; }
        public CrewViewModel[] Crew { get; set; }
    }

    public class CastPersonViewModel
    {
        public string Character { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }

    public class CrewViewModel
    {
        public string Department { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
    }

    public class GenreViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CompanyViewModel
    {
        public string LogoPath { get; set; }
        public string Name { get; set; }
        public string OriginCountry { get; set; }
    }

    public class CountryViewModel
    {
        public string Iso_3166_1 { get; set; }
        public string Name { get; set; }
    }

    public class SimilarMovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Popularity { get; set; }
        public string PosterPath { get; set; }
        public string BackdropPath { get; set; }
        public string Overview { get; set; }
        public string VoteAverage { get; set; }
    }

    public class TrailerViewModel
    {
        public string Id { get; set; }
        public string Iso_639_1 { get; set; }
        public string Iso_3166_1 { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Site { get; set; }
        public int Size { get; set; }
    }

    public class RatingViewModel
    {
        public string Source { get; set; }
        public string Value { get; set; }
    }
}