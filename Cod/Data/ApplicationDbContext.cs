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
                .HasKey(ab => new { ab.Id, ab.FollowingProfileId, ab.FollowedProfileId});

            modelbuilder.Entity<ProfileRequestsProfile>()
                .HasKey(ab => new { ab.Id, ab.RequestingProfileId, ab.RequestedProfileId});


            // definire relatii cu modelele profile si profile (fk)

            // HACK cand generam o migratie cu acest appdbcontext, in tabelele asociative se
            // mai adauga o coloana extra. momentan o stergem manual din codul migratiei.
            // de asemenea stergem spre final increment-urile

            modelbuilder.Entity<ProfileFollowsProfile>()
                .HasOne(ab => ab.FollowingProfile)
                .WithMany()
                .HasForeignKey(ab => ab.FollowingProfileId);

            modelbuilder.Entity<ProfileFollowsProfile>()
                .HasOne(ab => ab.FollowedProfile)
                .WithMany()
                .HasForeignKey(ab => ab.FollowedProfileId);

            modelbuilder.Entity<ProfileRequestsProfile>()
                .HasOne(ab => ab.RequestingProfile)
                .WithMany()
                .HasForeignKey(ab => ab.RequestingProfileId);

            modelbuilder.Entity<ProfileRequestsProfile>()
                .HasOne(ab => ab.RequestedProfile)
                .WithMany()
                .HasForeignKey(ab => ab.RequestedProfileId);
        }
    }
}