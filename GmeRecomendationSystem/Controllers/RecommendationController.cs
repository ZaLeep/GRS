using Microsoft.AspNetCore.Authorization;
using GmeRecomendationSystem.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GmeRecomendationSystem.Controllers
{
    [Authorize]
    [Route("[controller]")]
	public class RecommendationController : Controller
	{
        [Route("Index/{page?}")]
        public  IActionResult Index(int page = 1)
        {
            HomeController.UserContext(User.Identity as ClaimsIdentity, ViewData);
            ViewData["Count"] =  DBWork.GetCheckedItemsCount(Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]));

            return View( DBWork.GetRecomendedItems(Convert.ToInt32(ViewData["0"]), page, Convert.ToInt32(ViewData["4"])));
        }

        [Route("Search/{search?}/{page?}")]
        [Route("Search")]
        public  IActionResult Search(string search = "", int page = 1)
        {
            HomeController.UserContext(User.Identity as ClaimsIdentity, ViewData);
            ViewData["search"] = search;
            ViewData["Count"] =  DBWork.GetCheckedItemsCount(Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]));

            return View("Index",  DBWork.GetRecomendedItems(Convert.ToInt32(ViewData["0"]), search, page, Convert.ToInt32(ViewData["4"])));
        }

        [Route("Generate/")]
        public  IActionResult Generate()
        {
            HomeController.UserContext(User.Identity as ClaimsIdentity, ViewData);
            ViewData["Count"] =  DBWork.GetCheckedItemsCount(Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]));
            if (Convert.ToInt32(ViewData["Count"]) >= 5)
                 RecomendationCalculation.CreateRecommendation(Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]));
            return RedirectToAction("Index", "Recommendation");
        }
    }
}
