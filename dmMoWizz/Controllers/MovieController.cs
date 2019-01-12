using dmMoWizz.Models.Mongo;
using dmMoWizz.Models.ViewModels;
using dmMoWizz.Repositories;
using dmMoWizz.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dmMoWizz.Controllers
{
    public class MovieController : Controller
    {
        private MoviesRepository _moviesRepository;
        private readonly UserRepository _userRepository;

        private readonly RecommendationService _recommendationService;

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public MovieController()
        {
            _moviesRepository = new MoviesRepository();
            _userRepository = new UserRepository();

            _recommendationService = new RecommendationService();
        }
        // GET: Movies
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Details(int movieId)
        {
            var movieInfo = _moviesRepository.GetMovie(movieId);

            if (movieInfo == null)
            {
                return View("Error");
            }

            var cast = new List<CastPersonViewModel>();
            foreach (Cast castPerson in movieInfo.credits.cast)
            {
                cast.Add(new CastPersonViewModel
                {
                    Character = castPerson.character,
                    Name = castPerson.name,
                    Order = castPerson.order
                });
            }

            var crew = new List<CrewViewModel>();
            foreach (Crew castPerson in movieInfo.credits.crew)
            {
                crew.Add(new CrewViewModel
                {
                    Department = castPerson.department,
                    Name = castPerson.name,
                    Job = castPerson.job
                });
            }

            var backdrops = new List<ImageViewModel>();
            foreach (var backdrop in movieInfo.images.backdrops)
            {
                backdrops.Add(new ImageViewModel
                {
                    AspectRatio = backdrop.aspect_ratio,
                    FilePath = backdrop.file_path,
                    Height = backdrop.height,
                    Width = backdrop.width
                });
            }

            var posters = new List<ImageViewModel>();
            foreach (var poster in movieInfo.images.posters)
            {
                posters.Add(new ImageViewModel
                {
                    AspectRatio = poster.aspect_ratio,
                    FilePath = poster.file_path,
                    Height = poster.height,
                    Width = poster.width
                });
            }

            var genres = new List<GenreViewModel>();
            foreach (var genre in movieInfo.genres)
            {
                genres.Add(new GenreViewModel
                {
                    Id = genre.id,
                    Name = genre.name
                });
            }

            var companies = new List<CompanyViewModel>();
            foreach (var company in movieInfo.production_companies)
            {
                companies.Add(new CompanyViewModel
                {
                    LogoPath = company.logo_path,
                    Name = company.name,
                    OriginCountry = company.origin_country
                });
            }

            var countries = new List<CountryViewModel>();
            foreach (var country in movieInfo.production_countries)
            {
                countries.Add(new CountryViewModel
                {
                    Name = country.name,
                    Iso_3166_1 = country.iso_3166_1
                });
            }

            var similars = new List<SimilarMovieViewModel>();
            foreach (var similar in movieInfo.similar.results)
            {
                similars.Add(new SimilarMovieViewModel
                {
                    Id = similar.id,
                    Title = similar.title,
                    Overview = similar.overview,
                    BackdropPath = similar.backdrop_path,
                    Popularity = similar.popularity.ToString(),
                    PosterPath = similar.poster_path,
                    VoteAverage = similar.vote_average.ToString()
                });
            }

            var languages = new List<CountryViewModel>();
            foreach (var country in movieInfo.spoken_languages)
            {
                languages.Add(new CountryViewModel
                {
                    Name = country.name,
                    Iso_3166_1 = country.iso_639_1
                });
            }

            var trailers = new List<TrailerViewModel>();
            foreach (var trailer in movieInfo.videos.results.FindAll(v => v.type.Equals("Trailer")))
            {
                trailers.Add(new TrailerViewModel
                {
                    Id = trailer.id,
                    Iso_3166_1 = trailer.iso_3166_1,
                    Iso_639_1 = trailer.iso_639_1,
                    Key = trailer.key,
                    Name = trailer.name,
                    Site = trailer.site,
                    Size = trailer.size
                });
            }

            //TODO fill model
            var model = new MovieDetailsViewModel
            {
                Overview = movieInfo.overview,
                Id = movieInfo.id,
                BackdropPath = movieInfo.backdrop_path,
                ReleaseDate = movieInfo.release_date,
                Tagline = movieInfo.tagline,
                PosterPath = movieInfo.poster_path,
                Revenue = movieInfo.revenue.ToString(),
                Title = movieInfo.title,
                VoteAverage = movieInfo.vote_average.ToString(),
                VoteCount = movieInfo.vote_count,
                Budget = movieInfo.budget.ToString(),
                OriginalTitle = movieInfo.title,
                Popularity = movieInfo.popularity.ToString(),
                Credits = new CreditsViewModel
                {
                    Cast = cast.ToArray(),
                    Crew = crew.ToArray()
                },
                Backdrops = backdrops.ToArray(),
                Posters = posters.ToArray(),
                Homepage = movieInfo.homepage,
                OriginalLanguage = movieInfo.original_language,
                Status = movieInfo.status,
                Genres = genres.ToArray(),
                ProductionCompanies = companies.ToArray(),
                ProductionCountries = countries.ToArray(),
                SimilarMovies = similars.ToArray(),
                SpokenLanguages = languages.ToArray(),
                Trailers = trailers.ToArray()
            };

            return View(model);
        }

        public ActionResult Search()
        {
            var model = new SearchViewModel();

            //TODO make call to service which will fetch genres
            //TEST data
            model.GenresDropdown.Add(new GenreViewModel
            {
                Name = "Drama",
                Id = 18
            });
            model.GenresDropdown.Add(new GenreViewModel
            {
                Name = "Horror",
                Id = 27
            });

            return View("Search", model);
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel model)
        {
            var result = new List<SearchResultViewModel>();

            //TODO fill result

            if (model.Sort == "name")
            {
                result.OrderBy(x => x.Title);
            }
            else if (model.Sort == "year")
            {
                result.OrderBy(x => x.Year);
            }
            else if (model.Sort == "rate")
            {
                result.OrderBy(x => x.AverageVote);
            }

            return PartialView("Search", result);
        }

        public ActionResult Summary(int movieId)
        {
            var movieInfo = _moviesRepository.GetMovie(movieId);

            if (movieInfo == null)
            {
                return View("Error");
            }

            var cast = new List<CastPersonViewModel>();
            foreach (Cast castPerson in movieInfo.credits.cast)
            {
                cast.Add(new CastPersonViewModel
                {
                    Character = castPerson.character,
                    Name = castPerson.name,
                    Order = castPerson.order
                });
            }

            var user = _userRepository.Get(HttpContext.User.Identity.GetUserId());

            var model = new SummaryViewModel
            {
                Id = movieInfo.id,
                Title = movieInfo.title,
                AverageVote = movieInfo.vote_average.ToString(),
                Cast = cast.ToArray(),
                AddedToWatchlist = user.Watchlist.Contains(new WatchlistMovie { Id = movieInfo.id }),
                Year = movieInfo.release_date.Split('-')[0],
                Backdrop = "http://image.tmdb.org/t/p/w1280/" + movieInfo.backdrop_path,
                Overview = movieInfo.overview,
                PersonalRate = _recommendationService.GetRecommendation(movieId).ToString()
            };

            return PartialView("Summary", model);
        }
    }
}