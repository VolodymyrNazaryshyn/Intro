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
                    ).Include(a => a.Topic);
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
    }
}
