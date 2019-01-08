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

        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            if (!Request.IsAuthenticated)
            {
                return View();
            }

            var model = new HomePageViewModel();
            // TODO fill model
            //popular i suggested napuniti sa 8 filmova sortirati
            //sugested sortirati po personalrate a ostalo po average rate

            var movies = _movieService.GetPopular(8);
            foreach (MovieInfo movie in movies)
            {
                model.Popular.Add(new HomePageMovieViewModel
                {
                    AverageRate = movie.vote_average.ToString(),
                    Overview = movie.overview,
                    PersonalRate = "0",
                    Title = movie.title,
                    BackdropPath = "http://image.tmdb.org/t/p/w500/" + movie.backdrop_path,
                    PosterPath = "http://image.tmdb.org/t/p/w500/" + movie.poster_path
                });
            }

            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());
            List<KeyValuePair<string, int>> recommendedMovies = await _recommendationService.GetRecommendations(currentClaims);
            recommendedMovies.Sort((x, y) => y.Value.CompareTo(x.Value));
            int count = 0;
            foreach (var rm in recommendedMovies.ToList())
            {
                if (count == 8)
                {
                    break;
                }
                var movie = _movieRepository.GetMovieFromTitle(rm.Key);
                if (movie != null)
                {
                    count++;
                    model.Suggested.Add(new HomePageMovieViewModel
                    {
                        AverageRate = movie.vote_average.ToString(),
                        Overview = movie.overview,
                        PersonalRate = rm.Value.ToString(),
                        Title = movie.title,
                        BackdropPath = "http://image.tmdb.org/t/p/w500/" + movie.backdrop_path,
                        PosterPath = "http://image.tmdb.org/t/p/w500/" + movie.poster_path
                    });
                }
            }


            var user = _userRepository.Get(HttpContext.User.Identity.GetUserId());
            var sortedWatchlist = SortAndTakeNFirst(user.Watchlist, 4);
            foreach (WatchlistMovie w in sortedWatchlist)
            {
                var movie = _movieRepository.GetMovie(w.Id);
                model.Watchlist.Add(new HomePageMovieViewModel
                {
                    AverageRate = movie.vote_average.ToString(),
                    Overview = movie.overview,
                    PersonalRate = "0",
                    Title = movie.title,
                    BackdropPath = movie.backdrop_path,
                    PosterPath = movie.poster_path
                });
            }

            model.Suggested = model.Suggested.OrderByDescending(x => x.PersonalRate).ToList();
            model.Popular = model.Popular.OrderByDescending(x => x.AverageRate).ToList();
            //model.Watchlist = model.Watchlist.OrderByDescending(x => x.AverageRate).ToList();

            return View("AuthenticatedHome", model);
        }

        public ActionResult MoreSuggestedMovies()
        {
            //TODO implement logic

            //TEST DATA
            var model = new List<HomePageMovieViewModel>();

            var names = new string[]{"The Young Hedgehog", "Hedgehog Growing Up", "Hedgehog Fighting For Freedom", "Wild Hedgehogs",
                "Hedgehogs Land", "Hedgehogs Taking Space", "The Hedgehog King", "Hedgehog In The War Of Love",
                "Hedgehog and the Lobster", "Heavy Hedgehog", "The Hedgehogs Cars", "Over The Hedge"};

            for (int i = 0; i < 8; ++i)
            {
                var movie = new HomePageMovieViewModel
                {
                    AverageRate = "7.8",
                    Overview = "A movie of freedom and stupidity.",
                    PersonalRate = ((float)i / 8 * 100).ToString() + "%",
                    Title = names[i],
                    BackdropPath = "http://image.tmdb.org/t/p/w500//nJXlYXjbnno6tBDHqiW6ohkCrzQ.jpg",
                    PosterPath = "http://image.tmdb.org/t/p/w500//oYtkKiYx4ca1B8VvE8jxKCU7iN0.jpg"
                };

                model.Add(movie);
            }
            //zakomentirati ili obrisati test data

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