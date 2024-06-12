using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rantory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rantory.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Story> Stories { get; set; }
        public DbSet<Chapter> Chapters { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
    }
}
