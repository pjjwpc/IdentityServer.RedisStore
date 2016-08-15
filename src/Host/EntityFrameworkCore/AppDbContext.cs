using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace Host.EntityFrameworkCore {
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid> {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().ToTable("AspNetUsers");
            builder.Entity<IdentityRole<Guid>>().ToTable("AspNetRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("AspNetUserClaims");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AspNetRoleClaims");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("AspNetUserRoles");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("AspNetUserLogins");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("AspNetUserTokens");
        }
    }
}
