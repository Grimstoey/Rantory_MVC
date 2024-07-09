using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rantory.Models.ViewModels
{
    public class ChapterStoryVM
    {
        public Chapter Chapter { get; set; } = new();
        public Story Story { get; set; } = new();
        public int ChapterIndex { get; set; }
    }
}
