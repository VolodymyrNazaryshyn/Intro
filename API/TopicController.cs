using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Linq;

namespace Intro.API
{
    [Route("api/topic")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly DAL.Context.IntroContext _context;

        public TopicController(DAL.Context.IntroContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable Get() => _context.Topics.Include(t => t.Author);

        [HttpPost]
        public object Post([FromForm]Models.TopicModel topic)
        {
            if(topic == null)
            {
                return new { status = "Error", message = "No data" };
            }
            if(String.IsNullOrEmpty(topic.Title))
            {
                return new { status = "Error", message = "Empty Title" };
            }
            if(String.IsNullOrEmpty(topic.Description))
            {
                return new { status = "Error", message = "Empty Description" };
            }

            //// Задание: Добавить анализ заголовка Culture (xx-xx) uk-ua
            //String[] SupportedCultures = new string[] { "uk-ua", "en-gb" };
            //String CultureHeader = HttpContext.Request.Headers["Culture"].ToString().ToLower();
            //if (Array.IndexOf(SupportedCultures, CultureHeader) == -1)
            //{
            //    return new
            //    {
            //        status = "Error",
            //        message = "Culture Header empty or invalid or not supported"
            //    };
            //}
            //// Переносим информацию о локализации (культуре) в заголовки ответа
            //HttpContext.Response.Headers.Add("Culture", CultureHeader);

            // поиск пользователя в БД
            if (_context.Users.Find(topic.AuthorId) == null)
            {
                return new { status = "Error", message = "Forbidden" };
            }

            // Есть ли уже топик с таким названием?
            if (_context.Topics.Where(t => t.Title == topic.Title).Any())
            {
                return new { status = "Error", message = $"Topic '{topic.Title}' does exist" };
            }

            _context.Topics.Add(new()
            {
                Title = topic.Title,
                Description = topic.Description,
                AuthorId = topic.AuthorId
            });
           _context.SaveChanges();
            return new { status = "Ok" };
        }
    }
}
