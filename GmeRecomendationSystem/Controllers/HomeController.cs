using Microsoft.AspNetCore.Mvc;

namespace GmeRecomendationSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
