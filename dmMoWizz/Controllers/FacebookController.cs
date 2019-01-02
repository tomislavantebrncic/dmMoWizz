using dmMoWizz.Models.SocialMedia.Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
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
        private ApplicationUserManager _userManager;

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
        // GET: Facebook
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Likes()
        {
            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());

            var accessToken = currentClaims.FirstOrDefault(x => x.Type == "urn:tokens:facebook");

            if (accessToken == null)
            {
                return (new HttpStatusCodeResult(HttpStatusCode.NotFound, "Token not found"));
            }

            string url = String.Format(
                "https://graph.facebook.com/me?fields=id,name,likes.limit(1000){{category,name}}&access_token={0}", accessToken.Value);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";

            using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string result = await reader.ReadToEndAsync();

                dynamic jsonObj = System.Web.Helpers.Json.Decode(result);

               Info info = new Info(jsonObj);


                return View(info);
            }
        }

        public async Task<ActionResult> RecommendationsSimple()
        {
            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());

            var accessToken = currentClaims.FirstOrDefault(x => x.Type == "urn:tokens:facebook");

            if (accessToken == null)
            {
                return (new HttpStatusCodeResult(HttpStatusCode.NotFound, "Token not found"));
            }

            string url = String.Format(
                "https://graph.facebook.com/me?fields=id,name,likes.limit(1000){{category,name}}&access_token={0}", accessToken.Value);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";

            List<Like> likes;

            using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string result = await reader.ReadToEndAsync();

                dynamic jsonObj = System.Web.Helpers.Json.Decode(result);

                Info info = new Info(jsonObj);

                likes = info.Likes.Data;

            }



            return View();
        }

        #region Helper Methods


        #endregion
    }
}