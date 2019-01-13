using dmMoWizz.Models.Mongo;
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
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;

        private MoviesRepository _moviesRepository;

        private UserService _userService;
        private RecommendationService _recommendationService;

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

        public UserController()
        {
            _moviesRepository = new MoviesRepository();

            _userService = new UserService();
            _recommendationService = new RecommendationService();
        }

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task AddToWatchlist(int id)
        {
            var applicationUser = UserManager.FindById(User.Identity.GetUserId());

            await _userService.AddToWatchlistAsync(applicationUser.Id, id);
        }
        [HttpPost]
        public async Task RemoveFromWatchlist(int id)
        {
            var applicationUser = UserManager.FindById(User.Identity.GetUserId());

            await _userService.RemoveFromWatchlistAsync(applicationUser.Id, id);
        }


        [HttpPost]
        public async Task Rate(int movieId, int rating)
        {
            var applicationUser = UserManager.FindById(User.Identity.GetUserId());

            await _userService.RateAsync(applicationUser.Id, movieId, rating);
        }

        public ActionResult Watchlist()
        {
            var model = new List<WatchlistItemViewModel>();
            var applicationUser = UserManager.FindById(User.Identity.GetUserId());
            var userInfo = _userService.GetUserInfo(applicationUser.Id);

            foreach (var watchlistMovie in userInfo.Watchlist)
            {
                var movieInfo = _moviesRepository.GetMovie(watchlistMovie.Id);

                var cast = new List<CastPersonViewModel>();
                if (movieInfo.credits != null && movieInfo.credits.cast != null)
                    foreach (Cast castPerson in movieInfo.credits.cast)
                    {
                        cast.Add(new CastPersonViewModel
                        {
                            Character = castPerson.character,
                            Name = castPerson.name,
                            Order = castPerson.order
                        });
                    }

                model.Add(new WatchlistItemViewModel
                {
                    Id = movieInfo.id.ToString(),
                    Title = movieInfo.title,
                    AverageVote = movieInfo.vote_average.ToString(),
                    Overview = movieInfo.overview,
                    PosterURL = "http://image.tmdb.org/t/p/w500/" + movieInfo.poster_path,
                    Year = movieInfo.release_date.Split('-')[0],
                    AddedOnDate = watchlistMovie.DateAdded.ToShortDateString(),
                    // TO DO
                    PersonalRate = _recommendationService.GetRecommendation(movieInfo.id).ToString()+"%",
                    Cast = cast
                });
            }

            return View(model);
        }

        public ActionResult Recommendations()
        {
            return View();
        }

        public ActionResult RecommendationsScroll(int from, int to, int friendsConst, int similarConst, int rateConst)
        {
            //used for infnite scroll
            var model = new List<RecommendationViewModel>();
            model.Add(new RecommendationViewModel
            {
                Title = "The Tiny Hedgehog",
                AverageVote = "7.8",
                PersonalRate = "98%",
                Overview = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus a tortor placerat leo eleifend dapibus ac eu odio. In hac habitasse platea dictumst. Suspendisse tempor finibus sollicitudin. Donec faucibus diam vel dapibus dapibus. Maecenas turpis tellus, mollis vel odio eu, faucibus pharetra elit. Quisque bibendum, orci at elementum accumsan, felis metus finibus orci, vitae porttitor nulla erat sed turpis. Aenean accumsan aliquam vehicula. Etiam erat tellus, semper id nisi sit amet, finibus interdum ex. Sed sit amet massa pulvinar, dictum risus sollicitudin, hendrerit risus. Curabitur et libero vitae diam sodales aliquamuisque commodo in mi eu interdum.Sed dui libero,",
                Year="2018",
                PosterURL = "http://lorempixel.com/output/nightlife-q-c-640-480-5.jpg",
                Id = 217
            });
            model.Add(new RecommendationViewModel
            {
                Title = "The Tiny Hedgehog",
                AverageVote = "7.8",
                PersonalRate = "98%",
                Overview = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus a tortor placerat leo eleifend dapibus ac eu odio. In hac habitasse platea dictumst. Suspendisse tempor finibus sollicitudin. Donec faucibus diam vel dapibus dapibus. Maecenas turpis tellus, mollis vel odio eu, faucibus pharetra elit. Quisque bibendum, orci at elementum accumsan, felis metus finibus orci, vitae porttitor nulla erat sed turpis. Aenean accumsan aliquam vehicula. Etiam erat tellus, semper id nisi sit amet, finibus interdum ex. Sed sit amet massa pulvinar, dictum risus sollicitudin, hendrerit risus. Curabitur et libero vitae diam sodales aliquamuisque commodo in mi eu interdum.Sed dui libero,",
                Year = "2018",
                PosterURL = "http://lorempixel.com/output/nightlife-q-c-640-480-5.jpg",
                Id = 217
            });
            model.Add(new RecommendationViewModel
            {
                Title = "The Tiny Hedgehog",
                AverageVote = "7.8",
                PersonalRate = "98%",
                Overview = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus a tortor placerat leo eleifend dapibus ac eu odio. In hac habitasse platea dictumst. Suspendisse tempor finibus sollicitudin. Donec faucibus diam vel dapibus dapibus. Maecenas turpis tellus, mollis vel odio eu, faucibus pharetra elit. Quisque bibendum, orci at elementum accumsan, felis metus finibus orci, vitae porttitor nulla erat sed turpis. Aenean accumsan aliquam vehicula. Etiam erat tellus, semper id nisi sit amet, finibus interdum ex. Sed sit amet massa pulvinar, dictum risus sollicitudin, hendrerit risus. Curabitur et libero vitae diam sodales aliquamuisque commodo in mi eu interdum.Sed dui libero,",
                Year = "2018",
                PosterURL = "http://lorempixel.com/output/nightlife-q-c-640-480-5.jpg",
                Id = 217
            });
            model.Add(new RecommendationViewModel
            {
                Title = "The Tiny Hedgehog",
                AverageVote = "7.8",
                PersonalRate = "98%",
                Overview = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus a tortor placerat leo eleifend dapibus ac eu odio. In hac habitasse platea dictumst. Suspendisse tempor finibus sollicitudin. Donec faucibus diam vel dapibus dapibus. Maecenas turpis tellus, mollis vel odio eu, faucibus pharetra elit. Quisque bibendum, orci at elementum accumsan, felis metus finibus orci, vitae porttitor nulla erat sed turpis. Aenean accumsan aliquam vehicula. Etiam erat tellus, semper id nisi sit amet, finibus interdum ex. Sed sit amet massa pulvinar, dictum risus sollicitudin, hendrerit risus. Curabitur et libero vitae diam sodales aliquamuisque commodo in mi eu interdum.Sed dui libero,",
                Year = "2018",
                PosterURL = "http://lorempixel.com/output/nightlife-q-c-640-480-5.jpg",
                Id = 217
            });

            return PartialView("RecommendationsScroll", model);
        }

        public ActionResult GetRecommendationsFriends()
        {

            return View();
        }

        public ActionResult UserRatingsRecommendations()
        {

            return View();
        }
    }
}