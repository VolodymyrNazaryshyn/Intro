using Intro.DAL.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Intro.API
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IntroContext _context;
        private readonly Services.IHasher _hasher;

        public UserController(IntroContext context, Services.IHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        // GET: api/<UserController>
        [HttpGet]
        public string Get(string login, string password)
        {
            if (string.IsNullOrEmpty(login) && string.IsNullOrEmpty(password))
            {
                HttpContext.Response.StatusCode = 409;
                return "Conflict: login and password required";
            }

            if (string.IsNullOrEmpty(login))
            {
                HttpContext.Response.StatusCode = 409;
                return "Conflict: login required";
            }

            if (string.IsNullOrEmpty(password))
            {
                HttpContext.Response.StatusCode = 409;
                return "Conflict: password required";
            }

            DAL.Entities.User user =
                _context
                .Users
                .Where(u => u.Login == login)
                .FirstOrDefault();

            if (user == null)
            {
                HttpContext.Response.StatusCode = 401;
                return "Unauthorized: credentials rejected";
            }

            // Скопировано из AuthController - Login
            // Хешируем введенный пароль + соль
            String PassHash = _hasher.Hash(password + user.PassSalt);

            // Проверяем равенство полученного и хранимого хешей
            if (PassHash != user.PassHash)
            {
                HttpContext.Response.StatusCode = 401;
                return "Unauthorized: credentials rejected";
            }

            return user.Id.ToString();
        }

        // GET /api/user/f3b809a1-3b89-4538-aa95-08da6732bfa9
        [HttpGet("{id}")]
        public object Get(String id)
        {
            Guid guid;
            // Validation
            try
            {
                guid = Guid.Parse(id);
            }
            catch
            {
                HttpContext.Response.StatusCode = 409;
                return "Conflict: invalid id format (GUID required)";
            }
            // find user
            // return ( _context.Users.Find(guid) ?? new DAL.Entities.User() ) with { PassHash = "*", PassSalt = "*" };
            var user = _context.Users.Find(guid);
            if (user != null) return user with { PassHash = "*", PassSalt = "*" };
            return "null";
        }

        // POST api/<UserController>
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return $"POST {value}";
        }

        [HttpPut("{id}")]
        public object Put(String id, [FromForm] Models.RegUserModel userData)
        {
            DAL.Entities.User user;
            Guid userId;
            String avatarFileName = null;

            #region Validation
            try
            {
                userId = Guid.Parse(id); // проверка id на GUID
            }
            catch
            {
                HttpContext.Response.StatusCode = 409;
                return "Conflict: invalid id format (GUID required)";
            }

            user = _context.Users.Find(userId);
            if (user == null) // проверка на то, что существует пользователь с таким id
            {
                HttpContext.Response.StatusCode = 404;
                return $"User with id:{id} not found";
            }

            // поэтапная проверка какие из полей переданы в качестве изменений
            if (userData.RealName != null)
            {
                if (userData.RealName == String.Empty) // проверка на пустоту
                {
                    HttpContext.Response.StatusCode = 409;
                    return "Name couldn`t be empty!";
                }
                else if (!Regex.IsMatch(userData.RealName, @"^[A-Z][a-z]+ [A-Z][a-z]+$")) // серверная валидация имени
                {
                    HttpContext.Response.StatusCode = 409;
                    return $"{userData.RealName} doesn`t follow the pattern (example: Linus Torvalds)";
                }
            }

            if (userData.Login != null)
            {
                if (Regex.IsMatch(userData.Login, @"\s"))
                {
                    HttpContext.Response.StatusCode = 409;
                    return "Login couldn`t contain space(s)";
                }
                else if (_context.Users.Where(u => u.Login == userData.Login).Count() > 0)
                {
                    HttpContext.Response.StatusCode = 409;
                    return "Login in use";
                }
            }

            if (userData.Password1 != null)
            {
                if (userData.Password1 == String.Empty)
                {
                    return "Password couldn`t be empty";
                }
                else if (Regex.IsMatch(userData.Password1, @"\s"))
                {
                    return "Password couldn`t contain space(s)";
                }
                else if (!Regex.IsMatch(userData.Password1, @"(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*"))
                {
                    return "Password doesn`t follow the pattern: Minimum 8 characters, one digit, one uppercase letter and one lowercase letter";
                }
                else if (userData.Password1 != userData.Password2)
                {
                    return "Passwords don`t match";
                }
            }

            if (userData.Email != null)
            {
                if (userData.Email == String.Empty)
                {
                    return "Email couldn`t be empty";
                }
                else if (Regex.IsMatch(userData.Email, @"\s"))
                {
                    return "Email couldn`t contain space(s)";
                }
                else if (_context.Users.Where(u => u.Email == userData.Email).Count() > 0)
                {
                    return "Email in use";
                }
                else if (!Regex.IsMatch(userData.Email, @"^[A-z][A-z\d_]{3,16}@([a-z]{1,10}\.){1,5}[a-z]{2,3}$"))
                {
                    return userData.Email + @" doesn`t follow the pattern (example: volodimir@gmail.com, Alex@mail.odessa.ua)";
                }
            }

            // Avatar
            if (userData.Avatar != null)
            {
                // формируем имя для файла и сохраняем
                int pos = userData.Avatar.FileName.LastIndexOf('.');
                string IMG_Format = userData.Avatar.FileName.Substring(pos);
                avatarFileName = _hasher.Hash(Guid.NewGuid().ToString()) + IMG_Format;

                var file = new FileStream("./wwwroot/img/" + avatarFileName, FileMode.Create);
                userData.Avatar.CopyToAsync(file).ContinueWith(t => file.Dispose());

                if (user.Avatar != null) // если у пользователя есть Avatar
                {
                    // удаляем старый файл и заменяем у пользователя ссылку на новый файл
                    System.IO.File.Delete("./wwwroot/img/" + user.Avatar);
                }
            }
            #endregion

            // updates
            if (userData.RealName != null) user.RealName = userData.RealName; // подставляем в найденного пользователя
            if (userData.Login != null) user.Login = userData.Login;
            if (userData.Password1 != null && userData.Password1 == userData.Password2)
            {
                user.PassSalt = _hasher.Hash(DateTime.Now.ToString());
                user.PassHash = _hasher.Hash(userData.Password1 + user.PassSalt);
            }
            if (userData.Email != null) user.Email = userData.Email;
            if (userData.Avatar != null) user.Avatar = avatarFileName;

            _context.SaveChanges();

            return user; // возвращаем объект пользователя
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return $"DELETE {id}";
        }
    }
}
