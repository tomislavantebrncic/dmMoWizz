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



            return View();
        }
    }
}