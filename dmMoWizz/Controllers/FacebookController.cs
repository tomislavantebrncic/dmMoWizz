using dmMoWizz.Models.Mongo;
using dmMoWizz.Models.SocialMedia.Facebook;
using dmMoWizz.Repositories;
using dmMoWizz.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace dmMoWizz.Controllers
{
    [Authorize]
    public class FacebookController : Controller
    {
        private readonly MoviesRepository _moviesRepository;
        private ApplicationUserManager _userManager;
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

        public FacebookController()
        {
            _moviesRepository = new MoviesRepository();
            _recommendationService = new RecommendationService();
        }

        // GET: Facebook
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Likes()
        {
            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());

            Info info = await _recommendationService.GetInfoAsync(currentClaims, "https://graph.facebook.com/me?fields=id,name,likes.limit(1000){{category,name}}&access_token={0}");

            if (info == null)
            {
                return (new HttpStatusCodeResult(HttpStatusCode.NotFound, "Token not found"));
            }

            return View(info);
        }

        public async Task<ActionResult> Recommendations()
        {
            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());

            var info = await _recommendationService.GetInfoAsync(currentClaims, "https://graph.facebook.com/me?fields=id,name,likes.limit(100){{category,name}},friends.limit(1000){{likes.limit(1000){{category,name}}}}&access_token={0}");

            if (info == null)
            {
                return (new HttpStatusCodeResult(HttpStatusCode.NotFound, "Token not found"));
            }

            var sorted = _recommendationService.GetRecommendations(info);

            return View(sorted);
        }

        #region Helper Methods


        #endregion
    }
}