using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[PrimaryKey("IdOrder", "IdProductVariant")]
[Table("order_lines")]
[Index("IdProductVariant", Name = "fk_product_variants_id_order_lines_id_product_variant_idx")]
[Index("IdOrder", Name = "fk_orders_id_order_lines_id_order_idx")]
public class OrderLine
{
    [Key]
    [Column("id_order")]
    [Required]
    public int IdOrder { get; set; }

    [Key]
    [Column("id_product_variant")]
    [Required]
    public int IdProductVariant { get; set; }

    [Column("amount")]
    [Required]
    public int Amount { get; set; }

    [ForeignKey("IdOrder")]
    [InverseProperty("OrderLines")]
    public virtual Order IdOrderNavigation { get; set; }

    [ForeignKey("IdProductVariant")]
    [InverseProperty("OrderLines")]
    public virtual ProductVariant IdProductVariantNavigation { get; set; }
}
