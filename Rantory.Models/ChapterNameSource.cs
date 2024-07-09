using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rantory.Models
{
    public class ChapterNameSource
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "New chapter name")]
        public string Name { get; set; } = "";

        public int StoryId { get; set; }
        [ForeignKey("StoryId")]
        public Story? Story { get; set; }

        public int ChapterId { get; set; }
        [ForeignKey("ChapterId")]
        public Chapter? Chapter { get; set; }
    }
}
