using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rantory.Models
{
    public class User : IdentityUser
    {
        public ICollection<Story> Stories { get; set; }
    }
}
