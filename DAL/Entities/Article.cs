using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intro.DAL.Entities
{
    public class Article
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public string Text { get; set; }
        public Guid AuthorId { get; set; }
        public Guid? ReplyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PictureFile { get; set; }
    }
}
