using Microsoft.AspNetCore.Mvc;

namespace Intro.Controllers
{
    public class ForumController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
