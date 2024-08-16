using EventManagementSystem.Api.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Api.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            
            builder.HasData(new User
            {
                Id = 1,
                FirstName = "Adedeji",
                LastName = "Adeyemi",
                Email = "admin@test.ng",
                NormalizedEmail = "admin@test.ng".ToUpper(),
                UserName = "admin@test.ng",
                NormalizedUserName = "admin@test.ng".ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString("D"),
                PhoneNumber = "08031234567",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                LockoutEnd = null,
                AccessFailedCount = 0,
                PasswordHash = "AQAAAAEAACcQAAAAEHz9jeDAGD5NrInBBafBqFjW3XbnNG4w08PuNblIMvwdU1kzpGQd8mX3ca28HlBPkA=="
            });

        }
    }
}
