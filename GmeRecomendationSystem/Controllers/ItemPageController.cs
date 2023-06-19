using Microsoft.AspNetCore.Authorization;
using GmeRecomendationSystem.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using web = System.Web;

namespace GmeRecomendationSystem.Controllers
{
	[Authorize]
    [ApiController]
	[Route("[controller]")]
	public class ItemPageController : Controller
	{
		[Route("Index/{Id}")]
		public  IActionResult Index(int Id)
        {
            ViewData["Prev"] = Request.Headers["Referer"].ToString();
            HomeController.UserContext(User.Identity as ClaimsIdentity, ViewData);
            return View( DBWork.GetItem(Id, Convert.ToInt32(ViewData["4"]), Convert.ToInt32(ViewData["0"])));
        }

        [Route("AddReview/{Id}")]
        public IActionResult AddReview(int Id, int score)
        {
            ViewData["Prev"] = Request.Headers["Referer"].ToString();
            HomeController.UserContext(User.Identity as ClaimsIdentity, ViewData);
            DBWork.SetCheckedItem(Id, Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]), score);
            return View("Index", DBWork.GetItem(Id, Convert.ToInt32(ViewData["4"]), Convert.ToInt32(ViewData["0"])));
        }

        [Route("DeleteReview/{Id}")]
        public IActionResult DeleteReview(int Id)
        {
            ViewData["Prev"] = Request.Headers["Referer"].ToString();
            HomeController.UserContext(User.Identity as ClaimsIdentity, ViewData);
            DBWork.UnsetCheckedItem(Id, Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]));
            return View("Index", DBWork.GetItem(Id, Convert.ToInt32(ViewData["4"]), Convert.ToInt32(ViewData["0"])));
        }
    }
}
