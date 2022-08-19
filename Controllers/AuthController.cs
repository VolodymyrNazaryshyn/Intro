using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Intro.Controllers
{
    public class AuthController : Controller
    {
        private readonly Services.IHasher _hasher;
        private readonly DAL.Context.IntroContext _introContext;
        private readonly Services.RandomService _randomService;
        private readonly Services.IAuthService _authService;

        public AuthController(
            Services.IHasher hasher,
            DAL.Context.IntroContext introContext,
            Services.RandomService randomService,
            Services.IAuthService authService)
        {
            _hasher = hasher;
            _introContext = introContext;
            _randomService = randomService;
            _authService = authService;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            ViewData["AuthUser"] = _authService.User;            
        }

        public ViewResult Index()
        {
            // Сюда мы попадаем как при начале авторизации, так и в результате перенаправлений
            // из других методов. Каждый из них устанавливает свои сессионные атрибуты
            String LoginError = HttpContext.Session.GetString("LoginError");
            if (LoginError != null)
            {   // Был запрос на авторизацию (логин) и он завершился ошибкой
                ViewData["LoginError"] = LoginError;
                HttpContext.Session.Remove("LoginError");
            }
            String userId = HttpContext.Session.GetString("userId");
            if (userId != null)
            {   // Был запрос на авторизацию (логин) и он завершился успехом
                // находим пользователя по id и передаем найденный объект
                ViewData["AuthUser"] = _introContext.Users.Find(Guid.Parse(userId));
            }

            // Значение в HttpContext.Items добавлено в классе SessionAuthMiddleware
            ViewData["fromAuthMiddleware"] = HttpContext.Items["fromAuthMiddleware"];

            return View();
        }

        public ViewResult Register()
        {
            // извлекаем из сессии значение по ключу "RegErrors"
            String regErrors = HttpContext.Session.GetString("RegErrors");

            if (regErrors != null) // Есть сообщение
            {
                ViewData["regErrors"] = regErrors.Split(";"); // разделение строки на массив
                HttpContext.Session.Remove("RegErrors"); // удаляем из сессии - однократный вывод

                ViewData["saveData"] = HttpContext.Session.GetString("saveData").Split(";");
                HttpContext.Session.Remove("saveData");

                ViewData["validData"] = HttpContext.Session.GetString("validData").Split(";");
                HttpContext.Session.Remove("validData");
            }

            return View();
        }

        [HttpPost]                 // Срабатывает только на POST запрос
        public RedirectResult      // Возвращает перенаправление (302 Response status)
            Login(                 // Название метода
            String UserLogin,      // параметры связваются по именам
            String UserPassword)   // в форме должны быть такие же имена
        {
            // Валидация данных - проверка на пустоту и шаблоны
            // Для многократных проверок часто пользуются try-catch

            try
            {
                if (String.IsNullOrEmpty(UserLogin))
                    throw new Exception("Login empty");

                if (String.IsNullOrEmpty(UserPassword))
                    throw new Exception("Password empty");

                // проверяем (авторизируемся)
                // 1. По логину ищем пользователя и извлекаем соль, хеш пароля
                _authService.User =
                    _introContext
                    .Users
                    .Where(u => u.Login == UserLogin)
                    .FirstOrDefault();

                if (_authService.User == null) // нет пользователя с таким логином
                    throw new Exception("Login invalid");

                // 2. Хешируем введенный пароль + соль
                String PassHash = _hasher.Hash(UserPassword + _authService.User.PassSalt);

                // 3. Проверяем равенство полученного и хранимого хешей
                if (PassHash != _authService.User.PassHash)
                    throw new Exception("Password invalid");
            }
            catch (Exception ex)
            {   // сюда мы попадаем если была ошибка
                HttpContext.Session.SetString("LoginError", ex.Message);
                return Redirect("/Auth/Index"); // завершаем обработку
            }
            // Если не было catch, то и не было ошибок
            // тогда user - авторизованный пользователь, сохраняем его данные в сессии (обычно ограничиваются id)

            HttpContext.Session.SetString("userId", _authService.User.Id.ToString()); // регистрируем userId
            // создаем метку времени начала авторизации для контроля ее длительности
            HttpContext.Session.SetString("AuthMoment", DateTime.Now.Ticks.ToString());

            // Метод закончится установкой сессии LoginError либо userId и перенаправлением
            return Redirect("/"); //    /Home/Index - главная страница
        }

        public RedirectResult Logout()
        {
            HttpContext.Session.Remove("userId");
            // сохраняем время выхода в базу
            _authService.User.LogMoment = DateTime.Now;
            _introContext.SaveChanges();
            return Redirect("/");
        }

        private bool LoginExist(String login)
        {
            var listOfLogins = _introContext.Users.Select(u => u.Login).ToList();
            foreach (var log in listOfLogins)
                if (login == log) return true; // проверка логина на уникальность

            return false;
        }

        private bool EmailExist(String email)
        {
            var listOfEmails = _introContext.Users.Select(u => u.Email).ToList();
            foreach (var item in listOfEmails)
                if (email == item) return true; // проверка e-mail на уникальность

            return false;
        }

        [HttpPost] // Следующий метод срабатывает только на Post-запрос, метод может
        // автоматически собрать все переданные данные в модель по совпадении имен
        public RedirectToActionResult RegUser(Models.RegUserModel UserData)
        {
            // return Json(UserData); // способ проверить передачу данных

            String[] errors = new string[10]; // ошибки валидации
            String[] saveData = new string[5]; // имя, логин, пароли и e-mail
            bool isValidLogin = true;
            bool isValidEmail = true;
            bool isValidRealName = true;
            int errorCount = 0;

            if (UserData == null)
            {
                errors[0] = "Incorrect request (no data)";
                errorCount++;
            }
            else
            {
                if (String.IsNullOrEmpty(UserData.RealName))
                {
                    errors[1] = "Name couldn`t be empty";
                    errorCount++;
                }
                else if (!Regex.IsMatch(UserData.RealName, @"^[A-Z][a-z]+ [A-Z][a-z]+$")) // серверная валидация по типу "Имя Фамилия"
                {
                    errors[9] = "Doesn`t follow the pattern \"FirstName LastName\"";
                    isValidRealName = false;
                    errorCount++;
                }
                saveData[0] = UserData.RealName;

                if (String.IsNullOrEmpty(UserData.Login))
                {
                    errors[2] = "Login couldn`t be empty";
                    errorCount++;
                }
                else if (UserData.Login.Length < 5)
                {
                    errors[6] = "Login length must be at least 5 characters";
                    errorCount++;
                }
                else if (LoginExist(UserData.Login))
                {
                    errors[7] = "Such a login already exists";
                    isValidLogin = false;
                    errorCount++;
                }
                saveData[1] = UserData.Login;

                if (String.IsNullOrEmpty(UserData.Password1))
                {
                    errors[3] = "Password couldn`t be empty";
                    errorCount++;
                }
                saveData[3] = UserData.Password1;

                if (UserData.Password1 != UserData.Password2)
                {
                    errors[4] = "Passwords don`t match";
                    errorCount++;
                }
                saveData[4] = UserData.Password2;

                if (String.IsNullOrEmpty(UserData.Email))
                {
                    errors[5] = "E-mail couldn`t be empty";
                    errorCount++;
                }
                else if (EmailExist(UserData.Email))
                {
                    errors[8] = "Such an e-mail already exists";
                    isValidEmail = false;
                    errorCount++;
                }
                saveData[2] = UserData.Email;

                string newFileName = UserData.Avatar?.FileName;

                // есть переданный файл и нет ошибок
                if (UserData.Avatar != null && errorCount == 0)
                {
                    // убеждаемся, что в имени файла нету ../ (защита от DT)
                    if (newFileName.Contains("../")) newFileName = newFileName.Replace("../", "");

                    var pictures = new DirectoryInfo("./wwwroot/img/userImg/").GetFiles();

                    foreach (var picture in pictures)
                    {
                        if (newFileName == picture.Name)
                        {
                            string fileWithoutExt = Path.GetFileNameWithoutExtension(newFileName); // файл без расширения
                            string extension = Path.GetExtension(new FileInfo(newFileName).FullName); // расширение файла
                            newFileName = fileWithoutExt + Guid.NewGuid().ToString() + extension; // формируем новое имя файла
                        }
                    }
                    // сохраняем файл в папку wwwroot/img/userImg
                    UserData.Avatar.CopyToAsync(new FileStream("./wwwroot/img/userImg/" + newFileName, FileMode.Create));
                }

                // если валидация пройдена (нет сообщений об ошибках) - добавляем пользователя в БД
                if (errorCount == 0)
                {
                    var user = new DAL.Entities.User();
                    user.PassSalt = _hasher.Hash(DateTime.Now.ToString()); // крипто-соль - это случайное число (в строковом виде)
                    user.PassHash = _hasher.Hash(UserData.Password1 + user.PassSalt); // соль "смешивается" с паролем
                    user.Avatar = newFileName;
                    user.Email = UserData.Email;
                    user.RealName = UserData.RealName;
                    user.Login = UserData.Login;
                    user.RegMoment = DateTime.Now;

                    _introContext.Users.Add(user); // добавляем в БД (контекст)
                    _introContext.SaveChanges(); // сохраняем изменения

                    saveData[0] = null;
                    saveData[1] = null;
                    saveData[2] = null;
                }
            }

            String[] validData = new string[3];
            validData[0] = isValidLogin.ToString();
            validData[1] = isValidEmail.ToString();
            validData[2] = isValidRealName.ToString();

            // Сессия - "межзапросное хранилище", обычно сохраняют значимые типы
            HttpContext.Session.SetString("RegErrors", String.Join(";", errors));
            HttpContext.Session.SetString("saveData", String.Join(";", saveData));
            HttpContext.Session.SetString("validData", String.Join(";", validData));

            // return View("Register"); -- POST запрос не должен завершаться View
            return RedirectToAction("Register");
        }

        public IActionResult Profile()
        {
            // Задание: если пользователь не авторизован, то перенаправить на страницу логина
            if (_authService.User == null)
            {
                // Внутренний (внутрисерверный) редирект. В браузере остается адрес
                // /Auth/Profile, а реально отображается /Auth/Index
                // return View("Index");

                // Внешний редирект - браузер повторяет запрос на новый адрес
                // запрос /Auth/Profile -- редирект на /Auth/Index и он отображается
                return Redirect("/Auth/Index");
            }

            return View();
        }

        public String ChangeRealName(String NewName)
        {
            if (_authService.User == null) // если пользователь не авторизован
            {
                return "Unauthorized user"; // запрещенный доступ
            }
            if (_randomService.Integer % 2 == 0)
            {
                if (String.IsNullOrEmpty(NewName)) // проверка на пустоту
                {
                    return "Name couldn`t be empty!";
                }
                else if (!Regex.IsMatch(NewName, @"^[A-Z][a-z]+ [A-Z][a-z]+$")) // серверная валидация имени
                {
                    return $"{NewName} doesn`t follow the pattern (example: Linus Torvalds)";
                }
                else // не было ошибок
                {
                    // обновляем данные в БД
                    _authService.User.RealName = NewName;
                    _introContext.SaveChanges();
                    return "Name was updated!";
                }
            }

            return "Server error!";
        }

        [HttpPost]
        public JsonResult ChangeLogin([FromBody]String NewLogin)
        {
            String message = $"OK {NewLogin}";
            if (_authService.User == null) // если пользователь не авторизован
            {
                message = "Unauthorized user"; // запрещенный доступ
            }
            else if (String.IsNullOrEmpty(NewLogin))
            {
                message = "Login couldn`t be empty";
            }
            else if (Regex.IsMatch(NewLogin, @"\s"))
            {
                message = "Login couldn`t contain space(s)";
            }
            else if (_introContext.Users.Where(u => u.Login == NewLogin).Count() > 0)
            {
                message = "Login in use";
            }

            if (message == $"OK {NewLogin}") // не было ошибок
            {
                // обновляем данные в БД
                _authService.User.Login = NewLogin; 
                // _authService.User - ссылка на пользователя в БД,
                // поэтому изменения в _authService.User сразу отражаются на БД
                _introContext.SaveChanges(); // остается только сохранить изменения
            }
            return Json(message);
        }

        [HttpPut]
        public JsonResult ChangeEmail([FromForm] String NewEmail)
        {
            String message = $"OK {NewEmail}";
            if (_authService.User == null) // если пользователь не авторизован
            {
                message = "Unauthorized user"; // запрещенный доступ
            }
            else if (String.IsNullOrEmpty(NewEmail))
            {
                message = "Email couldn`t be empty";
            }
            else if (Regex.IsMatch(NewEmail, @"\s"))
            {
                message = "Email couldn`t contain space(s)";
            }
            else if (_introContext.Users.Where(u => u.Login == NewEmail).Count() > 0)
            {
                message = "Email in use";
            }
            else if (!Regex.IsMatch(NewEmail, @"^[A-z][A-z\d_]{3,16}@([a-z]{1,10}\.){1,5}[a-z]{2,3}$"))
            {
                message = NewEmail + @" doesn`t follow the pattern (example: volodimir@gmail.com, Alex@mail.odessa.ua)";
            }

            if (message == $"OK {NewEmail}") // не было ошибок
            {
                // обновляем данные в БД
                _authService.User.Email = NewEmail;
                _introContext.SaveChanges();
            }

            return Json(message);
        }

        [HttpPut]
        public JsonResult ChangePassword([FromForm] String NewPassword)
        {
            String message = "OK";
            if (_authService.User == null) // если пользователь не авторизован
            {
                message = "Unauthorized user"; // запрещенный доступ
            }
            else if (String.IsNullOrEmpty(NewPassword))
            {
                message = "Password couldn`t be empty";
            }
            else if (Regex.IsMatch(NewPassword, @"\s"))
            {
                message = "Password couldn`t contain space(s)";
            }
            else if (!Regex.IsMatch(NewPassword, @"(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*"))
            {
                message = "Password doesn`t follow the pattern: Minimum 8 characters, one digit, one uppercase letter and one lowercase letter";
            }

            if (message == "OK") // не было ошибок
            {
                // Хешируем введенный пароль + соль
                String PassHash = _hasher.Hash(NewPassword + _authService.User.PassSalt);
                // обновляем данные в БД
                _authService.User.PassHash = PassHash;
                _introContext.SaveChanges();
                message = "Password was updated!";
            }

            return Json(message);
        }

        [HttpPost]
        public JsonResult ChangeAvatar(IFormFile userAvatar)
        {
            if (_authService.User == null)
            {
                return Json(new { Status = "Error", Message = "Forbidden" });
            }
            if (userAvatar == null)
            {
                return Json(new { Status = "Error", Message = "No file" });
            }
            // формируем имя для файла и сохраняем
            int pos = userAvatar.FileName.LastIndexOf('.');
            string IMG_Format = userAvatar.FileName.Substring(pos);
            string ImageName = _hasher.Hash(Guid.NewGuid().ToString()) + IMG_Format;

            var file = new FileStream("./wwwroot/img/userImg/" + ImageName, FileMode.Create);
            userAvatar.CopyToAsync(file).ContinueWith(t => file.Dispose());

            if (_authService.User.Avatar != null) // если у пользователя есть Avatar
            {
                // удаляем старый файл и заменяем у пользователя ссылку на новый файл
                System.IO.File.Delete("./wwwroot/img/userImg/" + _authService.User.Avatar);
            }
      
            _authService.User.Avatar = ImageName;
            _introContext.SaveChanges();  // сохраняем изменения в БД

            return Json(new { Status = "Ok", Message = ImageName });
        }
    }
}
