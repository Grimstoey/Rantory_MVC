using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rantory.Models
{
    public class Story
    {
        [Key]
        public int Id { get; set; }

        public string? Title { get; set; } = "Untile";

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

        public bool FinishStatus { get; set; } = false;

    }
}
