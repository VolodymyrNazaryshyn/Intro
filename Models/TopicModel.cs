﻿using System;

namespace Intro.Models
{
    public class TopicModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AuthorId { get; set; }
    }
}
