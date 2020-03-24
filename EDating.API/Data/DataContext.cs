using EDating.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EDating.API.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Value> Values { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Photo> Photos {get;set;}
        public DbSet<Like> Likes {get;set;}
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //przekaznie do entity, że nasz klucz glowny bedzie sie sklada z tych dwoch kolumn
            builder.Entity<Like>()
                .HasKey( x => new {x.LikerId, x.LikeeId});

            builder.Entity<Like>()
                .HasOne(u => u.Likee)
                .WithMany( u => u.Likers)
                .HasForeignKey( u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(u => u.Liker)
                .WithMany( u => u.Likees)
                .HasForeignKey( u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            
        }
    }
}