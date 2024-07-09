using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rantory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Rantory.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Story> Stories { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<ChapterNameSource> ChapterNameSources { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Story - Chapter relationship
            builder.Entity<Chapter>()
                .HasOne(c => c.Story)
                .WithMany(s => s.Chapters)
                .HasForeignKey(c => c.StoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevents cascading deletes

            // Configure Story - User relationship
            builder.Entity<Story>()
                .HasOne(s => s.User)
                .WithMany(s => s.Stories)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevents cascading deletes

            // Configure Chapter - User relationship
            builder.Entity<Chapter>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevents cascading deletes
        }

    }
}
