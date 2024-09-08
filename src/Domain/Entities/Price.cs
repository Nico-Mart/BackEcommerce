using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[PrimaryKey("IdProduct", "CreatedAt")]
[Table("prices")]
[Index("IdProduct", Name = "fk_products_id_prices_id_product_idx")]
public class Price
{
    [Key]
    [Column("id_product")]
    [Required]
    public int IdProduct { get; set; }

    [Key]
    [Column("created_at", TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    [Column("price")]
    [Required]
    public int Value { get; set; }

    [ForeignKey("IdProduct")]
    [InverseProperty("Prices")]
    public virtual Product IdProductNavigation { get; set; }
}
