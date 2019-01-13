using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.Mongo
{
    public class BelongsToCollection
    {
        public int id { get; set; }
        public string name { get; set; }
        public string poster_path { get; set; }
        public string backdrop_path { get; set; }
    }

    public class Genre
    {
        public int id { get; set; }
        public string name { get; set; }

        public override bool Equals(object obj)
        {
            var genre = obj as Genre;
            return genre != null &&
                   id == genre.id;
        }

        public override int GetHashCode()
        {
            return 1877310944 + id.GetHashCode();
        }
    }

    public class ProductionCompany
    {
        public int id { get; set; }
        public string logo_path { get; set; }
        public string name { get; set; }
        public string origin_country { get; set; }
    }

    public class ProductionCountry
    {
        public string iso_3166_1 { get; set; }
        public string name { get; set; }
    }

    public class SpokenLanguage
    {
        public string iso_639_1 { get; set; }
        public string name { get; set; }
    }

    public class Keyword
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Keywords
    {
        public List<Keyword> keywords { get; set; }
    }

    public class Cast
    {
        public int cast_id { get; set; }
        public string character { get; set; }
        public string credit_id { get; set; }
        public int gender { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public string profile_path { get; set; }
    }

    public class Crew
    {
        public string credit_id { get; set; }
        public string department { get; set; }
        public int gender { get; set; }
        public int id { get; set; }
        public string job { get; set; }
        public string name { get; set; }
        public string profile_path { get; set; }
    }

    public class Credits
    {
        public List<Cast> cast { get; set; }
        public List<Crew> crew { get; set; }
    }

    public class Result
    {
        public bool adult { get; set; }
        public string backdrop_path { get; set; }
        public List<int> genre_ids { get; set; }
        public int id { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
        public string title { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public double popularity { get; set; }
    }

    public class Similar
    {
        public int page { get; set; }
        public List<Result> results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }

    public class Images
    {
        public List<object> backdrops { get; set; }
        public List<object> posters { get; set; }
    }

    public class MovieInfo
    {
        public bool adult { get; set; }
        public string backdrop_path { get; set; }
        public BelongsToCollection belongs_to_collection { get; set; }
        public int budget { get; set; }
        public List<Genre> genres { get; set; }
        public string homepage { get; set; }
        public int id { get; set; }
        public string imdb_id { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public double popularity { get; set; }
        public string poster_path { get; set; }
        public List<ProductionCompany> production_companies { get; set; }
        public List<ProductionCountry> production_countries { get; set; }
        public string release_date { get; set; }
        public Int64 revenue { get; set; }
        public int? runtime { get; set; }
        public List<SpokenLanguage> spoken_languages { get; set; }
        public string status { get; set; }
        public string tagline { get; set; }
        public string title { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public Keywords keywords { get; set; }
        public Credits credits { get; set; }
        public Similar similar { get; set; }
        public MovieImages images { get; set; }
        public List<Rating> Ratings { get; set; }
        public AppRating AppRating { get; set; }
        public List<Song> Soundtrack { get; set; }
        public Videos videos { get; set; }

        public override bool Equals(object obj)
        {
            var info = obj as MovieInfo;
            return info != null &&
                   id == info.id;
        }

        public override int GetHashCode()
        {
            return 1877310944 + id.GetHashCode();
        }
    }

    public class Video
    {
        public string id { get; set; }
        public string iso_639_1 { get; set; }
        public string iso_3166_1 { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string site { get; set; }
        public int size { get; set; }
        public string type { get; set; }
    }

    public class Videos
    {
        public List<Video> results { get; set; }
    }

    public class Song
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TunefindUrl { get; set; }
        public Artist Artist { get; set; }
    }

    public class Artist
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TunefindUrl { get; set; }
    }

    public class Rating
    {
        public string Source { get; set; }
        public string Value { get; set; }
    }

    public class AppRating
    {
        public int Count { get; set; }
        public int Rating { get; set; }
    }

    public class MovieImages
    {
        public int id { get; set; }
        public ImageFile[] posters { get; set; }
        public ImageFile[] backdrops { get; set; }
    }

    public class ImageFile
    {
        public string file_path { get; set; }
        public string aspect_ratio { get; set; }
        public int height { get; set; }
        public string iso_639_1 { get; set; }
        public string vote_average { get; set; }
        public int width { get; set; }
        public int vote_count { get; set; }
    }

}