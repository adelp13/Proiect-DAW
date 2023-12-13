using Cod.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Cod.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Group> Groups { get; set; }
        public DbSet<ApplicationUser> Profiles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // definirea relatiei many-to-many Profile urmareste profile

            base.OnModelCreating(modelBuilder);

            // definire primary key compus
            modelBuilder.Entity<Follows>()
                .HasKey(ab => new { ab.Id, ab.Id, ab.Id });


            // definire relatii cu modelele Profile si Profile (FK)

            modelBuilder.Entity<Follows>()
                .HasOne(ab => ab.Id)
                .WithMany(ab => ab.Follows)
                .HasForeignKey(ab => ab.Id);

            modelBuilder.Entity<Follows>()
                .HasOne(ab => ab.Id)
                .WithMany(ab => ab.Follows)
                .HasForeignKey(ab => ab.Id);
        }
        */
    
    }
}
