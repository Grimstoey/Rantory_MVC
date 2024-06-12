using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "Chapter")]
        public ICollection<string> ChapterName { get; set; } = [];

        public string? Content { get; set; }
    }
}
