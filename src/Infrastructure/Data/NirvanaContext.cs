using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class NirvanaContext : DbContext
{
    public NirvanaContext(DbContextOptions<NirvanaContext> options) : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderLine> OrderLines { get; set; }

    public virtual DbSet<Price> Prices { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductVariant> ProductVariants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Addresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_id_addresses_id_user");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.IdParentNavigation).WithMany(p => p.InverseIdParentNavigation).HasConstraintName("fk_categories_id_categories_id_parent");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_id_orders_id_user");
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasKey(e => new { e.IdOrder, e.IdProductVariant })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.OrderLines)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_id_order_lines_id_order");

            entity.HasOne(d => d.IdProductVariantNavigation).WithMany(p => p.OrderLines)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_variants_id_order_lines_id_product_variant");
        });

        modelBuilder.Entity<Price>(entity =>
        {
            entity.HasKey(e => new { e.IdProduct, e.CreatedAt })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Prices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_products_id_prices_id_product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_categories_id_products_id_category");
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.ProductVariants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_products_id_product_variants_id_product");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasDiscriminator(e => e.Role)
                .HasValue<Sysadmin>(Role.Sysadmin)
                .HasValue<Admin>(Role.Admin)
                .HasValue<Client>(Role.Client);

            entity.Property(e => e.Role)
                .HasConversion<int>();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        base.OnModelCreating(modelBuilder);
    }
}
