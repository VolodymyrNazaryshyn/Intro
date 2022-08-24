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

        public ArticleController(
            DAL.Context.IntroContext context,
            Services.IHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        //коллекция статей данного раздела(топика)
        [HttpGet("{id}")]
        public IEnumerable Get(string id) => _context.Articles
            .Include(a => a.Author)
            .Include(a => a.Topic)
            .Where(a => a.TopicId == Guid.Parse(id))
            .OrderBy(a => a.CreatedDate);

        [HttpPost]
        public object Post([FromForm]Models.ArticleModel article)
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
                CreatedDate = now,
                PictureFile = newFileName,
                Text = article.Text,
                ReplyId = article.ReplyId
            });

            // Обновляем таблицу Topic - дата последней статьи
            topic.LastArticleMoment = now;
            
            _context.SaveChanges();
            return new { status = "Ok" };
        }
    }
}
