using Microsoft.AspNetCore.Authorization;
using GmeRecomendationSystem.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GmeRecomendationSystem.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class LibraryController : Controller
	{
        [Route("Index/{page?}")]
        public  IActionResult Index(int page = 1)
        {
            int i = 0;
            foreach (var claim in HttpContext.User.Claims)
            {
                ViewData[Convert.ToString(i)] = claim.Value;
                i++;
            }

            return View( DBWork.GetLibraryItems(page, Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"])));
        }

        [Route("Search/{search?}/{page?}")]
        [Route("Search/")]
        public  IActionResult Search(string search = "", int page = 1)
        {
            int i = 0;
            foreach (var claim in HttpContext.User.Claims)
            {
                ViewData[Convert.ToString(i)] = claim.Value;
                i++;
            }
            ViewData["search"] = search;

            return View("Index",  DBWork.GetLibraryItems(search, page, Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"])));
        }
    }
}
