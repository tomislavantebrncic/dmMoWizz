﻿using dmMoWizz.Models.Mongo;
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

                var rate = userInfo.Ratings.FirstOrDefault(r => r.MovieId.Equals(movieInfo.id));
                
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
                    PersonalRate = _recommendationService.GetRecommendation(movieInfo.id).ToString()+"%",
                    Cast = cast,
                    
                    Rate = rate != null ? rate.Rating.ToString() : null
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

            var user = _userService.GetUserInfo(HttpContext.User.Identity.GetUserId());

            List<Recommendation> recommendations = _recommendationService.GetRecommendations(user, friendsConst, similarConst, rateConst);

            foreach (var recommendation in recommendations.Skip(from).Take(to))
            {
                var movieInfo = recommendation.Movie;
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

                var genres = new List<GenreViewModel>();
                foreach (var genre in movieInfo.genres)
                {
                    genres.Add(new GenreViewModel
                    {
                        Id = genre.id,
                        Name = genre.name
                    });
                }

                model.Add(new RecommendationViewModel
                {
                    Title = movieInfo.title,
                    AverageVote = movieInfo.vote_average.ToString(),
                    Id = movieInfo.id,
                    Cast = cast,
                    Overview = movieInfo.overview,
                    PersonalRate = recommendation.Rating + "%",
                    PosterURL = "http://image.tmdb.org/t/p/w500/" + movieInfo.poster_path,
                    Year = movieInfo.release_date.Split('-')[0],
                    Genres = genres
                });
            }

            System.Diagnostics.Debug.WriteLine(model.Count);

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