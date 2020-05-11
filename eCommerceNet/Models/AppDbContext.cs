using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace eCommerceNet.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        
        public DbSet<Comment> Comments { get; set; }

        public DbSet<CommentImage> CommentImages { get; set; }

        public DbSet<Product> Products { get; set; }
        
        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<ProductItem> ProductItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            foreach (var rel in mb.Model.GetEntityTypes().SelectMany(f => f.GetForeignKeys()))
            {
                rel.DeleteBehavior = DeleteBehavior.Restrict;
            }

            foreach (var prop in mb.Model.GetEntityTypes()
                                        .SelectMany(t => t.GetProperties())
                                        .Where(q => q.ClrType == typeof(decimal)))
            {
                prop.Relational().ColumnType = "decimal(18,2)";
            }

            base.OnModelCreating(mb);
        }
    }
}
