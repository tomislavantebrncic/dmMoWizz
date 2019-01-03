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
    }
}