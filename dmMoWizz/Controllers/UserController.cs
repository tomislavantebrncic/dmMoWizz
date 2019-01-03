using dmMoWizz.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dmMoWizz.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        private UserRepository _usersRepository;
        private MoviesRepository _moviesRepository;

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
            _usersRepository = new UserRepository();
            _moviesRepository = new MoviesRepository();
        }

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void AddToWatchlist(int id)
        {
            var applicationUser = UserManager.FindById(User.Identity.GetUserId());
            var userInfo = _usersRepository.Get(applicationUser.Id);

            userInfo.AddToWatchlist(id);

            _usersRepository.Update(userInfo);
        }


        [HttpPost]
        public void Rate(int movieId, int rating)
        {
            var applicationUser = UserManager.FindById(User.Identity.GetUserId());
            var userInfo = _usersRepository.Get(applicationUser.Id);

            int incRating = rating;
            int incCount = 1;
            if (userInfo.IsRatedMovie(movieId))
            {
                incCount = 0;
                incRating -= userInfo.Ratings.First(mr => mr.MovieId == movieId).Rating;
            }

            _moviesRepository.UpdateRating(movieId, incRating, incCount);
        }
    }
}