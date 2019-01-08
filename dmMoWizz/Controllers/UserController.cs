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
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;

        private UserService _userService;

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
            _userService = new UserService();
        }

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task AddToWatchlist(int id)
        {
            var applicationUser = UserManager.FindById(User.Identity.GetUserId());

            await _userService.AddToWatchlistAsync(applicationUser.Id, id);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task Rate(int movieId, int rating)
        {
            var applicationUser = UserManager.FindById(User.Identity.GetUserId());

            await _userService.RateAsync(applicationUser.Id, movieId, rating);
        }

        public ActionResult Watchlist()
        {
            var model = new List<WatchlistItemViewModel>();
            model.Add(new WatchlistItemViewModel
            {
                Title = "The Great Fall",
                AverageVote = "7.6",
                Id = "7",
                Overview = "The movie about fall one of the greatest emperors of all time. The movie about fall one of the greatest emperors of all time. The movie about fall one of the greatest emperors of all time.",
                PosterURL = "http://image.tmdb.org/t/p/w500//nJXlYXjbnno6tBDHqiW6ohkCrzQ.jpg",
                Year = "2010",
                PersonalRate = "77%",
                Cast = new List<CastPersonViewModel> { new CastPersonViewModel {
                        Character = "Mahthilda",
                        Name = "Jennifer Lawrence",
                        Order = 1
                    },
                    new CastPersonViewModel {
                        Character = "Adrianne",
                        Name = "Kate Olsen",
                        Order = 2
                    },
                    new CastPersonViewModel {
                        Character = "John",
                        Name = "Michael Douglas",
                        Order = 3
                    },
                }
            });
            model.Add(new WatchlistItemViewModel
            {
                Title = "The Great Fall",
                AverageVote = "7.6",
                Id = "7",
                Overview = "The movie about fall one of the greatest emperors of all time. The movie about fall one of the greatest emperors of all time. The movie about fall one of the greatest emperors of all time.",
                PosterURL = "http://image.tmdb.org/t/p/w500//nJXlYXjbnno6tBDHqiW6ohkCrzQ.jpg",
                Year = "2010",
                PersonalRate = "77%",
                Cast = new List<CastPersonViewModel> { new CastPersonViewModel {
                        Character = "Mahthilda",
                        Name = "Jennifer Lawrence",
                        Order = 1
                    },
                    new CastPersonViewModel {
                        Character = "Adrianne",
                        Name = "Kate Olsen",
                        Order = 2
                    },
                    new CastPersonViewModel {
                        Character = "John",
                        Name = "Michael Douglas",
                        Order = 3
                    },
                }
            });
            return View(model);
        }
    }
}