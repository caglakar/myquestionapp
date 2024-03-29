﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QandA.Data.Models
{
    public class QuestionPostRequest
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage ="Bir sıkıntı var! Content boş olamaz!")]
        public string Content { get; set; }
    }
}
