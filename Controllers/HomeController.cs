using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Intro.Services;

namespace Intro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RandomService _randomService;
        private readonly IHasher _hasher;
        private readonly IDateTime _dateTime;
        private readonly DAL.Context.IntroContext _introContext;
        private readonly IAuthService _authService;

        public HomeController(              // Внедрение зависимости через конструктор
            ILogger<HomeController> logger,
            RandomService randomService,
            IHasher hasher,
            IDateTime dateTime,
            DAL.Context.IntroContext introContext,
            IAuthService authService)
        {
            _logger = logger;
            _randomService = randomService;
            _hasher = hasher;
            _dateTime = dateTime;
            _introContext = introContext;
            _authService = authService;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            ViewData["AuthUser"] = _authService.User;
            // base.OnActionExecuting(context);
        }

        public ViewResult Index()
        {
            // ViewData - специальный динамический объект, служит для передачи данных между
            // Controller и View (кроме Model, которая основной передатчик данных)
            ViewData["rnd"] = _randomService.Integer;
            ViewBag.hash = _hasher.Hash("123"); // ViewBag - старая версия ViewData
            ViewData["date"] = _dateTime.Date();
            ViewData["time"] = _dateTime.Time();
            ViewData["UsersCount"] = _introContext.Users.Count();
            ViewData["UsersRealName"] = _introContext.Users.Select(u => u.RealName).ToList();
            // Значение в HttpContext.Items добавлено в классе SessionAuthMiddleware
            ViewData["fromAuthMiddleware"] = HttpContext.Items["fromAuthMiddleware"];
            // проверяем службу авторизации
            ViewData["authUserName"] = _authService.User?.RealName;

            return View(); // Controller переключает нас на View (возвращает Index.cshtml)
        }

        public ViewResult Privacy()
        {
            return View(); // Controller переключает нас на View (возвращает Privacy.cshtml)
        }

        public ViewResult About()
        {
            var model = new Models.AboutModel // создаем объект модели AboutModel
            {
                Data = "The Model Data" // подставляем данные
            };
            return View(model); // передаем данные на View
        }

        public async Task<IActionResult> Random()
        {
            return Content(_randomService.Integer.ToString());
        }

        public JsonResult Data()
        {
            return Json(new { field = "value" });
        }
        
        public ViewResult Contacts()
        {
            // создаем объект модели ContactsModel и подставляем в него данные
            var model = new Models.ContactsModel
            {
                Address = "271 Dominica Circle, Niceville, FL 32578, United States",
                Phone = "55-88-32",
                Name = "Microcomputer"
            };
            return View(model); // передаем данные на View
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public ViewResult Error()
        {
            return View(
                new Models.ErrorViewModel
                {
                    RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
        }
    }
}
