using dmMoWizz.Models.Mongo;
using dmMoWizz.Models.Recommendations;
using dmMoWizz.Models.ViewModels;
using dmMoWizz.Repositories;
using dmMoWizz.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace dmMoWizz.Controllers
{
    public class HomeController : Controller
    {
        private readonly MoviesRepository _movieRepository;
        private readonly UserRepository _userRepository;

        private readonly MovieService _movieService;
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

        public HomeController()
        {
            _movieRepository = new MoviesRepository();
            _userRepository = new UserRepository();
            _movieService = new MovieService();
            _recommendationService = new RecommendationService();
        }

        public async Task<ActionResult> Index()
        {
            if (!Request.IsAuthenticated)
            {
                return View();
            }

            var user = _userRepository.Get(HttpContext.User.Identity.GetUserId());

            var model = new HomePageViewModel();

            var movies = _movieService.GetPopular(8);
            foreach (MovieInfo movie in movies)
            {
                model.Popular.Add(new HomePageMovieViewModel
                {
                    Id = movie.id,
                    AverageRate = movie.vote_average.ToString(),
                    Overview = movie.overview,
                    Title = movie.title,
                    BackdropPath = "http://image.tmdb.org/t/p/w1280/" + movie.backdrop_path,
                    PosterPath = "http://image.tmdb.org/t/p/w1280/" + movie.poster_path,
                    AddedToWatchlist = user.Watchlist.Contains(new WatchlistMovie { Id = movie.id }),
                    PersonalRate = _recommendationService.GetRecommendation(movie.id) + "%"
                });
            }

            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());

            List<Recommendation> recommendedMovies = await _recommendationService.GetRecommendations(user, currentClaims, 1, 4, 1);
            foreach (var recommendation in recommendedMovies.Take(8).ToList())
            {
                var movie = recommendation.Movie;
                model.Suggested.Add(new HomePageMovieViewModel
                {
                    Id = movie.id,
                    AddedToWatchlist = user.Watchlist.Contains(new WatchlistMovie { Id = movie.id }),
                    AverageRate = movie.vote_average.ToString(),
                    Overview = movie.overview,
                    PersonalRate = Math.Round(recommendation.Rating, 2).ToString() + "%",
                    Title = movie.title,
                    BackdropPath = "http://image.tmdb.org/t/p/w500/" + movie.backdrop_path,
                    PosterPath = "http://image.tmdb.org/t/p/w500/" + movie.poster_path
                });
            }

            var sortedWatchlist = SortAndTakeNFirst(user.Watchlist, 4);
            foreach (WatchlistMovie w in sortedWatchlist)
            {
                var movie = _movieRepository.GetMovie(w.Id);
                model.Watchlist.Add(new HomePageMovieViewModel
                {
                    Id = movie.id,
                    AverageRate = movie.vote_average.ToString(),
                    Overview = movie.overview,
                    Title = movie.title,
                    BackdropPath = "http://image.tmdb.org/t/p/w500/" + movie.backdrop_path,
                    PosterPath = "http://image.tmdb.org/t/p/w500/" + movie.poster_path,
                    AddedToWatchlist = user.Watchlist.Contains(new WatchlistMovie { Id = movie.id }),
                    PersonalRate = _recommendationService.GetRecommendation(movie.id) + "%"
                });
            }

            model.Popular = model.Popular.OrderByDescending(x => x.AverageRate).ToList();

            return View("AuthenticatedHome", model);
        }

        public async Task<ActionResult> MoreSuggestedMovies()
        {
            var model = new List<HomePageMovieViewModel>();

            var user = _userRepository.Get(HttpContext.User.Identity.GetUserId());

            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());
            List<Recommendation> recommendedMovies = await _recommendationService.GetRecommendations(user, currentClaims, 1, 4, 1);
            foreach (var recommendation in recommendedMovies.Skip(8).Take(8).ToList())
            {
                var movie = recommendation.Movie;
                model.Add(new HomePageMovieViewModel
                {
                    AverageRate = movie.vote_average.ToString(),
                    Overview = movie.overview,
                    PersonalRate = recommendation.Rating.ToString(),
                    Title = movie.title,
                    BackdropPath = "http://image.tmdb.org/t/p/w500/" + movie.backdrop_path,
                    PosterPath = "http://image.tmdb.org/t/p/w500/" + movie.poster_path
                });
            }

            return PartialView("MoreSuggestedMovies", model);
        }

        #region Helper Methods

        private List<WatchlistMovie> SortAndTakeNFirst(HashSet<WatchlistMovie> watchlist, int n)
        {
            return watchlist.OrderByDescending(m => m.DateAdded).Take(n).ToList();
        }
        #endregion

    }
}