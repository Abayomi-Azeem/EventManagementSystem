using EventManagementSystem.Api.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static EventManagementSystem.Api.Common.AppEnums;

namespace EventManagementSystem.Api.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasData(new Role
            {
                Id = 1,
                Name = UserRoles.Admin.ToString(),
                NormalizedName = UserRoles.Admin.ToString().ToUpper()
            });
            builder.HasData(new Role
            {
                Id = 2,
                Name = UserRoles.Organizer.ToString(),
                NormalizedName = UserRoles.Organizer.ToString().ToUpper()
            });
            builder.HasData(new Role
            {
                Id = 3,
                Name = UserRoles.Participant.ToString(),
                NormalizedName = UserRoles.Participant.ToString().ToUpper()
            });
        }
    }
}
