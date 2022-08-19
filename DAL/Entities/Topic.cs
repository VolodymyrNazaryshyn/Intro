using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intro.DAL.Entities
{
    public class Topic
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public Guid AuthorId { get; set; }
        public Entities.User Author { get; set; }
    }
}
