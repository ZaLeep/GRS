using Microsoft.AspNetCore.Authorization;
using GmeRecomendationSystem.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GmeRecomendationSystem.Controllers
{
    [Authorize]
    [Route("[controller]")]
	public class RecommendationController : Controller
	{
        [Route("Index/{page?}")]
        public async Task<IActionResult> Index(int page = 1)
        {
            int i = 0;
            foreach (var claim in HttpContext.User.Claims)
            {
                ViewData[Convert.ToString(i)] = claim.Value;
                i++;
            }
            ViewData["search"] = "";
            ViewData["Count"] = await DBWork.GetCheckedItemsCount(Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]));

            return View(await DBWork.GetRecomendedItems(Convert.ToInt32(ViewData["0"]), page, Convert.ToInt32(ViewData["4"])));
        }

        [Route("Search/{search}/{page?}")]
        [Route("Search")]
        public async Task<IActionResult> Search(string search, int page = 1)
        {
            int i = 0;
            foreach (var claim in HttpContext.User.Claims)
            {
                ViewData[Convert.ToString(i)] = claim.Value;
                i++;
            }
            ViewData["search"] = search;
            ViewData["Count"] = await DBWork.GetCheckedItemsCount(Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]));

            return View("Index", await DBWork.GetRecomendedItems(Convert.ToInt32(ViewData["0"]), search, page, Convert.ToInt32(ViewData["4"])));
        }

        [Route("Generate/")]
        public async Task<IActionResult> Generate()
        {
            int i = 0;
            foreach (var claim in HttpContext.User.Claims)
            {
                ViewData[Convert.ToString(i)] = claim.Value;
                i++;
            }
            ViewData["Count"] = await DBWork.GetCheckedItemsCount(Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]));
            if (Convert.ToInt32(ViewData["Count"]) >= 5)
                await RecomendationCalculation.CreateRecommendation(Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]));
            return RedirectToAction("Index", "Recommendation");
        }
    }
}
