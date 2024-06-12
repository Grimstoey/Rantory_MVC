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

        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Strat")]
        public DateTime DateStart { get; set; }
        [Required]
        [Display(Name = "Finish")]
        public DateTime DateFinish { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }


        ICollection<Chapter> Chapters { get; set; }

    }
}
