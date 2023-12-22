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
        public DbSet<ProfileFollowsProfile> Follows { get; set; }
        public DbSet<ProfileRequestsProfile> FollowRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            // definirea relatiei many-to-many profile urmareste profile

            base.OnModelCreating(modelbuilder);

            // definire primary key compus
            modelbuilder.Entity<ProfileFollowsProfile>()
                .HasKey(ab => new { ab.Id, ab.FollowingProfileId, ab.FollowedProfileId });

            modelbuilder.Entity<ProfileRequestsProfile>()
                .HasKey(ab => new { ab.Id, ab.RequestingProfileId, ab.RequestedProfileId });

            // de asemenea stergem spre final increment-urile

            modelbuilder.Entity<ProfileFollowsProfile>()
                .HasOne(ab => ab.FollowedProfile)
                .WithMany(ab => ab.Follows)
                .HasForeignKey(ab => ab.FollowedProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            modelbuilder.Entity<ProfileRequestsProfile>()
                .HasOne(ab => ab.RequestedProfile)
                .WithMany(ab => ab.Requests)
                .HasForeignKey(ab => ab.RequestedProfileId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}