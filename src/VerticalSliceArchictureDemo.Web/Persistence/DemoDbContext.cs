using Microsoft.EntityFrameworkCore;
using VerticalSliceArchictureDemo.Web.Domain.Entities;

namespace VerticalSliceArchictureDemo.Web.Persistence
{
    public class DemoDbContext : DbContext
    {
        protected DemoDbContext()
        {
        }

        public DemoDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DemoDbContext).Assembly);
        }
    }
}
