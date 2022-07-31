using Intro.DAL.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Intro.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IntroContext _context;

        public ArticleController(IntroContext context)
        {
            _context = context;
        }

        // CREATE - POST
        [HttpPost]
        public object Post([FromForm]Models.ArticleModel article)
        {
            // проверено - при отсутствии заголовка возвращает "" (не Exception)
            string AuthorIdHeader = HttpContext.Request.Headers["Author-Id"].ToString();
            Guid AuthorId;
            Guid TopicId;
            // Validation
            try
            { AuthorId = Guid.Parse(AuthorIdHeader); }
            catch
            {
                return new
                {
                    status = "Error",
                    message = "Author-Id Header empty or invalid (GUID expected)"
                };
            }

            // проверяем данные
            if(article == null)
            {
                return new { status = "Error", message = "No data" };
            }

            try
            {
                TopicId = Guid.Parse(article.TopicId);
            }
            catch
            {
                return new { status = "Error", message = "Topic-Id invalid (GUID expected)" };
            }

            if(String.IsNullOrEmpty(article.Text) || String.IsNullOrEmpty(article.PictureFile.FileName))
            {
                return new { status = "Error", message = "Empty Text or Picture File" };
            }

            // поиск пользователя в БД
            var author = _context.Users.Find(AuthorId);
            if(author == null)
            {
                return new
                {
                    status = "Error",
                    message = "Forbidden"
                };
            }

            string newFileName = article.PictureFile?.FileName;

            // есть переданный файл
            if(article.PictureFile != null)
            {
                // убеждаемся, что в имени файла нету ../ (защита от DT)
                if(newFileName.Contains("../"))
                    newFileName = newFileName.Replace("../", "");

                var pictures = new DirectoryInfo("./wwwroot/img/articleImg").GetFiles();

                foreach(var picture in pictures)
                {
                    if(newFileName == picture.Name)
                    {
                        string fileWithoutExt = Path.GetFileNameWithoutExtension(newFileName); // файл без расширения
                        string extension = Path.GetExtension(new FileInfo(newFileName).FullName); // расширение файла
                        newFileName = fileWithoutExt + Guid.NewGuid().ToString() + extension; // формируем новое имя файла
                    }
                }
                // сохраняем файл в папку wwwroot/img
                article.PictureFile.CopyToAsync(new FileStream("./wwwroot/img/articleImg/" + newFileName, FileMode.Create));
            }

            // Создаем статью и сохраняем в БД
            _context.Articles.Add(new()
            {
                TopicId = TopicId,
                Text = article.Text,
                AuthorId = author.Id,
                ReplyId = article?.ReplyId,
                CreatedDate = DateTime.Now,
                PictureFile = article.PictureFile.FileName
            });
            _context.SaveChanges();

            return new { status = "Ok", message = $"Article created. Text: {article.Text}" };
        }

        //коллекция статей данного раздела(топика)
        [HttpGet]
        public IEnumerable<DAL.Entities.Article> Get(String TopicId)
        {
            return _context.Articles.Where(a => a.TopicId.ToString() == TopicId);
        }
    }
}
