using Microsoft.AspNetCore.Mvc;

namespace Intro.Controllers
{
    public class ForumController : Controller
    {
        private readonly Services.IAuthService _authService;

        public ForumController(Services.IAuthService authService)
        {
            _authService = authService;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            ViewData["AuthUser"] = _authService.User;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Topic(string id)
        {
            ViewData["id"] = id;

            return View();
        }
    }
}
