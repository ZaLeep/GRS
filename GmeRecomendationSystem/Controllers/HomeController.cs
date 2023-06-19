using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Authorization;
using GmeRecomendationSystem.Models;
using GmeRecomendationSystem.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GmeRecomendationSystem.Controllers
{
    public class HomeController : Controller
    {
		[HttpGet]
		public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoginPage()
        {
            return Redirect("~/#login");
        }

        [Route("Home/GoTo/{loc}")]
        public  IActionResult GoTo(string loc)
        {
            if(!(loc.Equals("about")) && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }
            return Redirect("~/#" + loc);
        }

        [HttpPost]
        public IActionResult LogIn(HomeFormsModel m)
        {
            ModelState.Remove("RegisterModel");
            if (ModelState.IsValid)
            {
                int? emailCount =  DBWork.CheckEmailExistence(m.LogInModel.Email);
                if (emailCount == 0)
                {
                    ViewBag.Error = "У базі ще не існує такої пошти.";
                    return Redirect("~/#login");
                }
                if (emailCount == -1)
                {
                    return View();
                }
                UserModel user =  DBWork.LogIn(m.LogInModel.Email, m.LogInModel.Password);
                if (user == null)
                    return View();
                if(user.Id == 0)
                {
                    ViewBag.Error = "Пароль не відповідає введеній пошті.";
                    return Redirect("~/#login");
                }

                 Authenticate(user);
                return RedirectToAction("Index", "Profile");
            }
            return View();
        }
        
        public IActionResult Register(HomeFormsModel m)
        {
            ModelState.Remove("LogInModel");
            if (ModelState.IsValid)
            {
                UserModel user =  DBWork.Registrate(m.RegisterModel.NickName, m.RegisterModel.Email, m.RegisterModel.Password);
                if (user.Id == 0)
                {
                    return Redirect("~/#register");
                }
                else
                {
                     Authenticate(user);
                    return RedirectToAction("Index", "Profile");
                }
            }
            return Redirect("~/#register");
        }
        
        [HttpPost]
        public JsonResult CheckRegisterEmail(HomeFormsModel m)
        {
            int? emailCount = DBWork.CheckEmailExistence(m.RegisterModel.Email);
            if (emailCount == 0)
                return Json(true);
            if(emailCount == 1)
                return Json(false);
            return Json("Вибачайте: проблеми із базою даних.");
        }

        private void Authenticate(UserModel user)
        {
            SubjectRangeModel SAModel = DBWork.GetSubjectRange();
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
                new Claim("UserName", user.UserName),
                new Claim("Email", user.UserEmail),
                new Claim("SAID", SAModel.SAID.ToString())
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
             HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [Authorize]
        public  IActionResult Logout()
        {
             HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public static void UserContext(ClaimsIdentity identity, ViewDataDictionary ViewData)
        {
            int i = 0;
            foreach (var claim in identity.Claims)
            {
                ViewData[Convert.ToString(i)] = claim.Value;
                i++;
            }
        }
    }
}
