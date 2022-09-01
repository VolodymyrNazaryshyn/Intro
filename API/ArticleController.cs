using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace Intro.API
{
    [Route("api/article")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly DAL.Context.IntroContext _context;
        private readonly Services.IHasher _hasher;
        private readonly Services.IAuthService _authService;

        public ArticleController(
            DAL.Context.IntroContext context,
            Services.IHasher hasher,
            Services.IAuthService authService)
        {
            _context = context;
            _hasher = hasher;
            _authService = authService;
        }

        [HttpGet]
        public IEnumerable Get()
        {
            // GET-параметры, передаваемые в запросе (после ?)
            // собираются в коллекции HttpContext.Request.Query
            // доступны через индексатор Query["key"]
            if(HttpContext.Request.Query["del"] == "true")
            {
                // запрос удаленных статей
                // проверяем аутентификацию
                if(_authService.User == null)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return new string[0];
                }

                return _context.Articles
                    .Where(
                        a => _authService.User.Id == a.AuthorId && a.DeleteMoment != null
                    ).Include(a => a.Topic).OrderBy(a => a.DeleteMoment);
            }

            return new string[0];
        }

        //коллекция статей данного раздела(топика)
        [HttpGet("{id}")]
        public IEnumerable Get(string id)
        {
            Guid topicId = Guid.Parse(id);
            /*
            var list = _context
                .Articles
                .Include(a => a.Author)
                .Include(a => a.Topic)
                .Include(a => a.Reply)
                .Where(a => a.TopicId == topicId)
                //.Include(a => a.Author)
                //.Include(a => a.Topic)
                //.Include(a => a.Reply)
                .Where(a => a.TopicId == topicId && a.ReplyId == null)
                .OrderBy(a => a.CreatedDate)
                .ToList();
            foreach(Article ar in list)
            {
                ar.Replies = _context
                .Articles
                .Where(a =>  a.ReplyId == ar.Id)
                .OrderBy(a => a.CreatedDate)
                .ToList();
                // foreach (Article ar2 in ar.Replies)
            }
            */
            return _context
                .Articles
                .Include(a => a.Author)
                .Include(a => a.Topic)
                .Include(a => a.Reply)
                .Where(a => a.TopicId == topicId && a.DeleteMoment == null)
                .OrderBy(a => a.CreatedMoment);
        }

        [HttpPost]
        public object Post([FromForm] Models.ArticleModel article)
        {
            if(article == null)
            {
                return new { status = "Error", message = "No data" };
            }
            if(String.IsNullOrEmpty(article.Text))
            {
                return new { status = "Error", message = "Empty Text" };
            }

            // Есть ли пользователь с переданным ID?
            if(_context.Users.Find(article.AuthorId) == null)
            {
                return new { status = "Error", message = "Invalid Author" };
            }
            // Есть ли раздел (топик) с переданным ID?
            var topic = _context.Topics.Find(article.TopicId);
            if(topic == null)
            {
                return new { status = "Error", message = "Invalid Topic" };
            }
            // Есть ли ответ (replyId) и проверка на существование
            if(article.ReplyId != null)
            {
                if(_context.Articles.Find(article.ReplyId) == null)
                {
                    return new { status = "Error", message = "Invalid Reply Id" };
                }
            }

            string newFileName = null;

            if(article.Picture != null) // Есть ли переданный файл?
            {
                int pos = article.Picture.FileName.LastIndexOf('.');
                if(pos == -1)  // У файла нет расширения
                {
                    return new { status = "Error", message = "No extension" };
                }
                string ext = article.Picture.FileName.Substring(pos);
                if(Array.IndexOf(new String[] { ".png", ".jpg", ".bmp", ".gif" }, ext) == -1)
                {
                    return new { status = "Error", message = "Invalid file format" };
                }

                newFileName = _hasher.Hash(Guid.NewGuid().ToString()) + ext;

                var pictures = new DirectoryInfo("./wwwroot/img/articleImg").GetFiles();
                foreach(var picture in pictures)
                {
                    if(newFileName == picture.Name)
                    {
                        newFileName = _hasher.Hash(newFileName) + ext;
                    }
                }

                var file = new FileStream("./wwwroot/img/articleImg/" + newFileName, FileMode.Create);
                article.Picture.CopyToAsync(file).ContinueWith(t => file.Dispose());
            }

            DateTime now = DateTime.Now;

            _context.Articles.Add(new()
            {
                AuthorId = article.AuthorId,
                TopicId = article.TopicId,
                CreatedMoment = now,
                PictureFile = newFileName,
                Text = article.Text,
                ReplyId = article.ReplyId
            });

            // Обновляем таблицу Topic - дата последней статьи
            topic.LastArticleMoment = now;

            _context.SaveChanges();
            return new { status = "Ok" };
        }

        [HttpPut("{id}")]
        public object Put(string id, [FromBody]string text)
        {
            // text не пустой
            if(String.IsNullOrEmpty(text))
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { Status = "error", message = "Empty text not allowed" };
            }
            // id валидный
            Guid articleId;
            try
            {
                articleId = Guid.Parse(id);
            }
            catch
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new { Status = "error", message = "Invalid id format (GUID required)" };
            }
            var article = _context.Articles.Find(articleId);
            if(article == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return new { Status = "error", message = "Article not found" };
            }
            if(article.DeleteMoment != null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                return new { Status = "error", message = "Deleted Article should not be edited" };
            }

            // авторизация
            if(_authService.User == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "error", message = "Log in for editing" };
            }

            // юзер - автор
            if(_authService.User.Id != article.AuthorId)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                return new { Status = "error", message = "Only authors could edit articles" };
            }

            // фиксируем изменения
            article.Text = text;
            _context.SaveChanges();

            return new { Status = "Ok", message = "Edit complete" };
        }

        [HttpDelete("{id}")]
        public object Delete(string id)
        {
            // Проверка аутентификации
            if(_authService.User == null)
            {
                return new { status = "Error", message = "Anauthorized" };
            }

            // Проверка id на валидность
            Guid articleId;
            try
            {
                articleId = Guid.Parse(id);
            }
            catch
            {
                return new { status = "Error", message = "Invalid id" };
            }
            var article = _context.Articles.Find(articleId);
            if(article == null)
            {
                return new { status = "Error", message = "Invalid article" };
            }

            // Проверка того что удаляемый пост принадлежит автору
            if(article.AuthorId != _authService.User.Id)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                return new { status = "Error", message = "Forbidden" };
            }

            // удаление - установка DeleteMoment для статьи
            article.DeleteMoment = DateTime.Now;
            _context.SaveChanges();

            return new { status = "Ok", message = "Deleted" };
        }

        // Public метод без атрибутов - как метод по умолчанию,
        // сюда попадут запросы, которые не подошли под остальные методы
        // Такой метод должен быть только один (иначе исключение -
        // неоднозначность выбора метода), название произвольное (не
        // обязательно Default)
        public object Default([FromQuery]string uid)
        {
            // Метод запроса можно узнать в HttpContext.Request.Method
            // сюда попадают все методы, в т.ч. нестандартные
            switch(HttpContext.Request.Method)
            {
                case "PURGE": return Purge(uid);
                case "UNLINK": return Unlink(uid);
            }
            return new { uid, HttpContext.Request.Method };
        }

        // Приватные методы не сканнируются контекстом, но могут быть
        // вызваны из метода по умолчанию
        private object Purge(string uid)
        {
            String userId = HttpContext.Request.Headers["User-Id"].ToString();
            // Валидность uid, userId
            var error = "Error";
            var message = "";
            Guid articleId, authorId;
            try
            {
                articleId = Guid.Parse(uid);
                authorId = Guid.Parse(userId);
            }
            catch
            {
                message = "Invalid id structure";
                return new { error, message };
            }

            // наличие в БД этих id
            var author = _context.Users.Find(authorId);
            if(author == null)
            {
                message = "Author not found";
                return new { error, message };
            }

            var article = _context.Articles.Find(articleId);
            if(article == null)
            {
                message = "Article not found";
                return new { error, message };
            }

            // статья действительно удаленная
            if(article.DeleteMoment == null)
            {
                message = "Article not deleted";
                return new { error, message };
            }

            // соответствие автора статьи и юзера
            if(article.AuthorId != authorId)
            {
                message = "Author rejected";
                return new { error, message };
            }

            // Восстанавливаем статью
            article.DeleteMoment = null;
            _context.SaveChanges();

            message = "Ok";
            return new { message };
        }

        private object Unlink(string uid)
        {
            return new { uid, Method = "Unlink" };
        }
    }
}
