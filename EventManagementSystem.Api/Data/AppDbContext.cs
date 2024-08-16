using EventManagementSystem.Api.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EventManagementSystem.Api.Data
{
    public class AppDbContext: IdentityDbContext<User, Role, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {                
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            var tableNameWithAspNet = builder.Model.GetEntityTypes().Where(e => e.GetTableName().StartsWith("AspNet")).ToList();
            tableNameWithAspNet.ForEach(x =>
            {
                x.SetTableName(x.GetTableName().Substring(6));
            });
        }
    }
}
