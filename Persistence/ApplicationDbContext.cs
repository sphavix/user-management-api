using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoleBasedUserManagementApi.Models;

namespace RoleBasedUserManagementApi.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {

        public DbSet<TokenInfo> TokenInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" });


            //Seed Data
            var hasher = new PasswordHasher<IdentityUser>();
            var admin = new IdentityUser
            {
                UserName = "rolebasedadmin@geeking.com",
                NormalizedUserName = "ROLEBASEDADMIN@GEEKING.COM",

                Email = "rolebasedadmin@geeking.com",
                NormalizedEmail = "ROLEBASEDADMIN@GEEKING.COM",

                PhoneNumber = "0812761542",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LockoutEnabled = false
            };

            admin.PasswordHash = hasher.HashPassword(admin, "StrongPassword123");

            builder.Entity<IdentityUser>().HasData(admin);

            // Assign role to admin user
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "1",
                    UserId = admin.Id
                });

        }
    }
}
