using Microsoft.EntityFrameworkCore;

namespace Login.Models
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions options) : base(options) 
        { 
        
        }

        public DbSet<Users>users { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
