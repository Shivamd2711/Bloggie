using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRoleId = "94c5a1bc-4671-414c-b5ad-cec5ad1e4143";
            var superAdminRoleId = "39169c86-bf88-462d-b208-e5a3ea78e0e8";
            var userRoleId = "1df85d5c-eedd-4e88-aada-88e704a3aab7";

            //seed the roles (user, admin, superadmin)
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "Admin",
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId
                },
                new IdentityRole
                {
                    Name = "SuperAdmin",
                    NormalizedName = "SuperAdmin",
                    Id = superAdminRoleId,
                    ConcurrencyStamp = superAdminRoleId

                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "User",
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId
                }
            };


            builder.Entity<IdentityRole>().HasData(roles);
            //List of roles are inserted into builder object. 
            //when we run ef db migration these roles taken as seed and will be inserted into database

            //seed superadmin
            var superAdminId = "c05d7ff3-b1b0-40d1-880d-2e8f3b7401d0";
            var superAdmingUser = new IdentityUser
            {
                UserName = "superadmin@bloggie.com",
                Email = "superadmin@bloggie.com",
                NormalizedEmail = "superadmin@bloggie.com".ToUpper(),
                NormalizedUserName = "superadmin@bloggie.com".ToUpper(),
                Id = superAdminId
            };

            //create password
            superAdmingUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(superAdmingUser, "SuperAdmin@123");

            //seeding super admin user in builder
            builder.Entity<IdentityUser>().HasData(superAdmingUser);


            //add all the roles to super admin user
            var superAdminRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>()
                {
                    RoleId = adminRoleId,
                    UserId = superAdminId
                },
                 new IdentityUserRole<string>()
                {
                    RoleId = superAdminRoleId,
                    UserId = superAdminId
                },
                  new IdentityUserRole<string>()
                {
                    RoleId = userRoleId,
                    UserId = superAdminId
                }
            };
            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoles);

        }
    }
}
