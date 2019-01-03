using dmMoWizz.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dmMoWizz.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return View();
            }

            var model = new HomePageViewModel();
            // TODO fill model
            //popular i suggested napuniti sa 8 filmova sortirati
            //watchlist napuniti sa 4
            //sugested sortirati po personalrate a ostalo po average rate
            
            //TEST DATA
            var names = new string[]{"The Young Hedgehog", "Hedgehog Growing Up", "Hedgehog Fighting For Freedom", "Wild Hedgehogs",
                "Hedgehogs Land", "Hedgehogs Taking Space", "The Hedgehog King", "Hedgehog In The War Of Love",
                "Hedgehog and the Lobster", "Heavy Hedgehog", "The Hedgehogs Cars", "Over The Hedge"};

            for (int i = 0; i < 8; ++i)
            {
                var movie = new HomePageMovieViewModel
                {
                    AverageRate = "7.8",
                    Overview = "A movie ab freedom and stupidity.",
                    PersonalRate = ((float)i / 8 * 100).ToString() + "%",
                    Title = names[i],
                    BackdropPath = "http://image.tmdb.org/t/p/w500//nJXlYXjbnno6tBDHqiW6ohkCrzQ.jpg",
                    PosterPath = "http://image.tmdb.org/t/p/w500//oYtkKiYx4ca1B8VvE8jxKCU7iN0.jpg"
                };

                model.Popular.Add(movie);
                model.Suggested.Add(movie);
                model.Watchlist.Add(movie);
            }
            //zakomentirati ili obrisati test data

            model.Suggested = model.Suggested.OrderByDescending(x => x.PersonalRate).ToList();
            model.Popular = model.Popular.OrderByDescending(x => x.AverageRate).ToList();
            model.Watchlist = model.Watchlist.OrderByDescending(x => x.AverageRate).ToList();

            return View("AuthenticatedHome", model);
        }

    }
}