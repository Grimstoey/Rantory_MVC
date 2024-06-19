using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rantory.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Story> Stories { get; set; }

        public string? ProfileImg { get; set; }
    }
}
