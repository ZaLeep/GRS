using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using GmeRecomendationSystem.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GmeRecomendationSystem.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ProfileController : Controller
	{
        [Route("Index/{page?}")]
        public  IActionResult Index(int page = 1)
		{
			int i = 0;
            var identity = User.Identity as ClaimsIdentity;
            foreach (var claim in identity.Claims)
			{
				ViewData[Convert.ToString(i)] = claim.Value;
				i++;
            }
            ViewData["SA"] =  DBWork.GetSubjectRanges();
            ViewData["SAID"] = Convert.ToInt32(ViewData["4"]);


            return View( DBWork.GetCheckedItems(Convert.ToInt32(ViewData["0"]), page, Convert.ToInt32(ViewData["4"])));
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
            ViewData["SA"] =  DBWork.GetSubjectRanges();

            return View("Index",  DBWork.GetCheckedItems(Convert.ToInt32(ViewData["0"]), search, page, Convert.ToInt32(ViewData["4"])));
        }

        [Route("ChangeSA")]
        public  IActionResult ChangeSA(int sa)
        {
            int i = 0;
            var identity = User.Identity as ClaimsIdentity;
            foreach (var claim in identity.Claims)
            {
                ViewData[Convert.ToString(i)] = claim.Value;
                i++;
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, ViewData["0"].ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, ViewData["1"].ToString()),
                new Claim("UserName", ViewData["2"].ToString()),
                new Claim("Email", ViewData["3"].ToString()),
                new Claim("SAID", sa.ToString())
            };
             HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsIdentity id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
             HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

            return RedirectToAction("Index", "Profile");
        }
    }
}
