using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rantory.Models.ViewModels
{
    public class UserChapterVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<Chapter> Chapters { get; set; }
    }
}
