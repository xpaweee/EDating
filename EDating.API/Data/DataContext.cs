using EDating.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EDating.API.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Value> Values { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
    }
}