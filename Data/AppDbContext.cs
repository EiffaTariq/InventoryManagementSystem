using IMS.Models;
using Microsoft.EntityFrameworkCore;
namespace IMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<POLineItem> POLineItems { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<LowStockAlert> LowStockAlerts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
                entity.Property(p => p.Description)
                .HasMaxLength(100);
                entity.Property(p => p.UnitPrice)
                .HasColumnType("decimal(18,2)");
                //Relationships
                entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete Category if products exist
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
                entity.Property(c => c.Description)
                .HasMaxLength(100);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);
                entity.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(150);
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.HasIndex(u => u.Email)
                      .IsUnique();
                entity.Property(u => u.FullName)
                     .IsRequired()
                     .HasMaxLength(100);

                entity.Property(u => u.Role)
                      .IsRequired()
                      .HasMaxLength(20);
            });
            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.Property(po => po.Status)
                .HasConversion<int>();     // explicit: enum stored as int

                entity.HasKey(po => po.Id);

                entity.HasOne(po => po.Supplier)
                .WithMany(po => po.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(po => po.CreatedByUser)
                .WithMany()
                .HasForeignKey(po => po.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<POLineItem>(entity =>
            {
                entity.Property(li => li.UnitPrice)
                      .HasColumnType("decimal(18,2)");     // avoid EF Core decimal truncation warning
            });
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasKey(sm => sm.Id);

                entity.HasOne(sm => sm.Product)
                      .WithMany(p => p.StockMovements)
                      .HasForeignKey(sm => sm.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sm => sm.PurchaseOrder)
                      .WithMany()
                      .HasForeignKey(sm => sm.PurchaseOrderId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false); // nullable FK
            });

            modelBuilder.Entity<LowStockAlert>(entity =>
            {
                entity.HasKey(la => la.Id);

                entity.HasOne(la => la.Product)
                      .WithMany()
                      .HasForeignKey(la => la.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
