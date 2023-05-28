using Microsoft.AspNetCore.Authorization;
using GmeRecomendationSystem.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GmeRecomendationSystem.Controllers
{
	[Authorize]
	[Route("[controller]")]
	public class ItemPageController : Controller
	{
		[Route("Index/{Id}")]
		public  IActionResult Index(int Id)
		{
            int i = 0;
            foreach (var claim in HttpContext.User.Claims)
            {
                ViewData[Convert.ToString(i)] = claim.Value;
                i++;
            }
            return View( DBWork.GetItem(Id, Convert.ToInt32(ViewData["4"]), Convert.ToInt32(ViewData["0"])));
        }

        [Route("AddReview/{Id}")]
        public  IActionResult AddReview(int Id, int score)
        {
            int i = 0;
            foreach (var claim in HttpContext.User.Claims)
            {
                ViewData[Convert.ToString(i)] = claim.Value;
                i++;
            }
             DBWork.SetCheckedItem(Id, Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]), score);
            return View("Index",  DBWork.GetItem(Id, Convert.ToInt32(ViewData["4"]), Convert.ToInt32(ViewData["0"])));
        }

        [Route("DeleteReview/{Id}")]
        public  IActionResult DeleteReview(int Id)
        {
            int i = 0;
            foreach (var claim in HttpContext.User.Claims)
            {
                ViewData[Convert.ToString(i)] = claim.Value;
                i++;
            }
             DBWork.UnsetCheckedItem(Id, Convert.ToInt32(ViewData["0"]), Convert.ToInt32(ViewData["4"]));
            return View("Index",  DBWork.GetItem(Id, Convert.ToInt32(ViewData["4"]), Convert.ToInt32(ViewData["0"])));
        }
    }
}
