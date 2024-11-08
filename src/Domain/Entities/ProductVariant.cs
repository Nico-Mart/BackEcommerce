using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("product_variants")]
[Index("IdProduct", Name = "fk_products_id_product_variants_id_product_idx")]
public class ProductVariant
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("id_product")]
    [Required]
    public int IdProduct { get; set; }

    [Column("stock")]
    [Required]
    public int Stock { get; set; }

    [Column("size")]
    [StringLength(32)]
    public string? Size { get; set; }

    [Column("color")]
    [StringLength(16)]
    public string? Color { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("IdProduct")]
    [InverseProperty("ProductVariants")]
    public virtual Product IdProductNavigation { get; set; }

    [InverseProperty("IdProductVariantNavigation")]
    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
}
