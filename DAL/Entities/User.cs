﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Intro.DAL.Entities 
{
    public record User 
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public String RealName { get; set; }
        public String Login { get; set; }

        [JsonIgnore]
        public String PassHash { get; set; }
        [JsonIgnore]
        public String PassSalt { get; set; }

        public String Email { get; set; }
        public String Avatar { get; set; }
        public DateTime RegMoment { get; set; }
        public DateTime? LogMoment { get; set; }

    }
}
