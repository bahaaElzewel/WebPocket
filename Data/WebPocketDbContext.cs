using Microsoft.EntityFrameworkCore;
using WebPocket.Models;

namespace WebPocket.Data
{
    public class WebPocketDbContext : DbContext
    {
        public WebPocketDbContext(DbContextOptions  options) : base(options)
        {
            
        }

        public DbSet<Pockets> Pockets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}