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
        public IEnumerable Get() => _context.Topics
            .Include(t => t.Author).OrderBy(t => t.LastArticleMoment);

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

            DateTime now = DateTime.Now;

            _context.Topics.Add(new()
            {
                Title = topic.Title,
                Description = topic.Description,
                AuthorId = topic.AuthorId,
                CreatedTime = now,
                LastArticleMoment = now
            });

           _context.SaveChanges();

            return new { status = "Ok" };
        }
    }
}
