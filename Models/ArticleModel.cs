using Microsoft.AspNetCore.Http;
using System;

namespace Intro.Models
{
    public class ArticleModel
    {
        public string TopicId { get; set; }
        public string Text { get; set; }
        public Guid? ReplyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public IFormFile PictureFile { get; set; }
    }
}
