using Microsoft.AspNetCore.Authorization;
using GmeRecomendationSystem.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GmeRecomendationSystem.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class LibraryController : Controller
	{
        [Route("Index/{page?}")]
        public  IActionResult Index(int page = 1)
        {
            HomeController.UserContext(User.Identity as ClaimsIdentity, ViewData);

            return View( DBWork.GetLibraryItems(page, Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"])));
        }

        [Route("Search/{search?}/{page?}")]
        [Route("Search/")]
        public  IActionResult Search(string search = "", int page = 1)
        {
            HomeController.UserContext(User.Identity as ClaimsIdentity, ViewData);
            ViewData["search"] = search;

            return View("Index",  DBWork.GetLibraryItems(search, page, Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"])));
        }
    }
}
