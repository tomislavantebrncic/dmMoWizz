using dmMoWizz.Models.Mongo;
using dmMoWizz.Models.ViewModels;
using dmMoWizz.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dmMoWizz.Controllers
{
    public class MovieController : Controller
    {
        private MoviesRepository _moviesRepository;

        public MovieController()
        {
            _moviesRepository = new MoviesRepository();
        }
        // GET: Movies
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Details(int movieId)
        {
            var movieInfo = _moviesRepository.GetMovie(movieId);

            if (movieInfo == null)
            {
                return View("Error");
            }
            var model = new MovieDetailsViewModel
            {
                Budget = movieInfo.budget.ToString(),
                OriginalTitle = movieInfo.title,
                Popularity = movieInfo.popularity.ToString()
            };

            //model.Budget = "15574737";
            //model.OriginalTitle = "test film";
            //model.Popularity = "5";
            //remove later

            return View(model);
        }

        public ActionResult FindMovieByName(string name)
        {
            var movies = _moviesRepository.GetMovies(name);

            var model = new List<SearchResultViewModel>();

            foreach (var movie in movies)
            {
                var cast = new List<CastPersonViewModel>();
                foreach (Cast castPerson in movie.credits.cast)
                {
                    cast.Add(new CastPersonViewModel
                    {
                        Character = castPerson.character,
                        Name = castPerson.name,
                        Order = castPerson.order
                    });
                }

                model.Add(new SearchResultViewModel
                {
                    Title = movie.title,
                    AverageVote = movie.vote_average.ToString(),
                    Id = movie.id.ToString(),
                    Overview = movie.overview,
                    PosterURL = movie.poster_path,
                    Year = movie.release_date.Split('-')[0],
                    Cast = cast
                });
            }

            return View("Search", model);
        }
    }
}