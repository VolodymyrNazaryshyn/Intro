using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        public DateTime CreatedTime { get; set; }
        public DateTime LastArticleMoment { get; set; }

        [JsonIgnore]
        public List<Article> Articles { get; set; }
    }
}
