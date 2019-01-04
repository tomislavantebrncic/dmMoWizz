using dmMoWizz.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dmMoWizz.Controllers
{
    public class MoviesController : Controller
    {
        // GET: Movies
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Details(int movieId)
        {
            //TODO fill data

            //TEST DATA
            var model = new MovieDetailsViewModel();

            model.Budget = "15574737";
            model.OriginalTitle = "test film";
            model.Popularity = "5";
            //remove later

            return View(model);
        }

        public ActionResult FindMovieByName(string name)
        {
            //TODO implement searching 

            //TEST DATA 
            var model = new List<SearchResultViewModel>();
            model.Add(new SearchResultViewModel
            {
                Title = "The Great Fall",
                AverageVote = "7.6",
                Id = "7",
                Overview = "The movie about fall one of the greatest emperors of all time. The movie about fall one of the greatest emperors of all time. The movie about fall one of the greatest emperors of all time.",
                PosterURL = "http://image.tmdb.org/t/p/w500//nJXlYXjbnno6tBDHqiW6ohkCrzQ.jpg",
                Year = "2010",
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
            model.Add(new SearchResultViewModel
            {
                Title = "The Small Fall",
                AverageVote = "1.6",
                Id = "7",
                Overview = "The movie about fall one of the greatest emperors of all time. The movie about fall one of the greatest emperors of all time. The movie about fall one of the greatest emperors of all time.",
                PosterURL = "http://image.tmdb.org/t/p/w500//nJXlYXjbnno6tBDHqiW6ohkCrzQ.jpg",
                Year = "2010",
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
            // remove or delete test data after implementation

            return View("Search", model);
        }
    }
}