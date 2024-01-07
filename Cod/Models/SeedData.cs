using Cod.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cod.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (!context.Roles.Any()) {
                    context.Roles.AddRange(
                        new IdentityRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7210", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                        new IdentityRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7211", Name = "User", NormalizedName = "User".ToUpper() }
                    );
                }

                if (!context.Users.Any())
                {
                    var hasher = new PasswordHasher<ApplicationUser>();
                    context.Users.AddRange(
                        new ApplicationUser
                        {
                            // primary key
                            Id = "8e445865-a24d-4543-a6c6-9443d048cdb0",
                            UserName = "admin1@test.com",
                            EmailConfirmed = true,
                            NormalizedEmail = "ADMIN1@TEST.COM",
                            Email = "admin1@test.com",
                            NormalizedUserName = "ADMIN1@TEST.COM",
                            PasswordHash = hasher.HashPassword(null, "Admin1!"),
                            FirstName = "Admin",
                            LastName = "Adminescu",
                            isPrivate = true
                        },
                        new ApplicationUser
                        {
                            // primary key
                            Id = "8e445865-a24d-4543-a6c6-9443d048cdb1",
                            UserName = "user1@test.com",
                            EmailConfirmed = true,
                            NormalizedEmail = "USER1@TEST.COM",
                            Email = "user1@test.com",
                            NormalizedUserName = "USER1@TEST.COM",
                            PasswordHash = hasher.HashPassword(null, "User1!"),
                            FirstName = "User",
                            LastName = "Userescu",
                            isPrivate = false
                        }
                    );
                }

                if (!context.UserRoles.Any())
                {
                    context.UserRoles.AddRange(
                        new IdentityUserRole<string>
                        {
                            RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                            UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
                        },
                        new IdentityUserRole<string>
                        {
                            RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                            UserId = "8e445865-a24d-4543-a6c6-9443d048cdb1"
                        }
                    );
                }
                
                if (!context.Follows.Any()) {
                    context.Follows.AddRange(
                        new ProfileFollowsProfile
                        {
                            FollowingProfileId = "8e445865-a24d-4543-a6c6-9443d048cdb0",
                            FollowedProfileId = "8e445865-a24d-4543-a6c6-9443d048cdb1"
                        },
                        new ProfileFollowsProfile
                        {
                            FollowingProfileId = "8e445865-a24d-4543-a6c6-9443d048cdb1",
                            FollowedProfileId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
                        }
                    );
                }
                context.SaveChanges();
            }
        }
    }
}
