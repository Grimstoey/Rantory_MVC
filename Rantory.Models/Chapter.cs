﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rantory.Models
{
    public class Chapter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Chapter name")]
        public string ChapterName { get; set; } = "";

        public string? Content { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public int StoryId { get; set; }
        [ForeignKey("StoryId")]
        public Story? Story { get; set; }


    }
}
