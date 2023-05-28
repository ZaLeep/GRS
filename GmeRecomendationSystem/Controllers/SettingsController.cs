using Microsoft.AspNetCore.Authorization;
using GmeRecomendationSystem.Models;
using GmeRecomendationSystem.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GmeRecomendationSystem.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class SettingsController : Controller
    {
        [Route("Index/")]
        public  IActionResult Index()
        {
            HomeController.UserContext(User.Identity as ClaimsIdentity, ViewData);
            ViewData["SA"] =  DBWork.GetSubjectRanges(true);
            ViewData["SAID"] = Convert.ToInt32(ViewData["4"]);
            if (ViewData["1"].ToString().Equals("admin"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Profile");
            }
        }

        [Route("AddSA/")]
        public  IActionResult AddSA(SubjectRangeModel model)
        {
            HomeController.UserContext(User.Identity as ClaimsIdentity, ViewData);
            if (ViewData["1"].ToString().Equals("admin"))
            {
                 DBWork.AddSubjectRange(model);
                return RedirectToAction("Index", "Settings");
            }
            else
            {
                return RedirectToAction("Index", "Profile");
            }
        }

        [Route("SetSA/{sa}/{inWork?}")]
        [Route("SetSA/{sa}")]
        [Route("SetSA")]
        public  IActionResult SetSA(int sa, string inWork = "off")
        {
            int i = 0;
            foreach (var claim in HttpContext.User.Claims)
            {
                ViewData[Convert.ToString(i)] = claim.Value;
                i++;
            }
            if (ViewData["1"].ToString().Equals("admin"))
            {
                 DBWork.SetSubjectRangeUse(sa, inWork.Equals("on") ? 1 : 0);
                return RedirectToAction("Index", "Settings");
            }
            else
            {
                return RedirectToAction("Index", "Profile");
            }
        }
    }
}
