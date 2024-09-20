using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("products")]
[Index("IdCategory", Name = "fk_categories_id_products_id_category_idx")]
[Index("Name", Name = "name_UNIQUE", IsUnique = true)]
public class Product
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("id_category")]
    [Required]
    public int IdCategory { get; set; }

    [Column("name")]
    [Required]
    [StringLength(64)]
    public string Name { get; set; }

    [Column("description")]
    [Required]
    [StringLength(255)]
    public string Description { get; set; }

    [Column("imageurl")]
    [StringLength(512)]
    public string? ImageUrl {  get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("IdCategory")]
    [InverseProperty("Products")]
    public virtual Category IdCategoryNavigation { get; set; }

    [InverseProperty("IdProductNavigation")]
    public virtual ICollection<Price> Prices { get; set; } = new List<Price>();

    [InverseProperty("IdProductNavigation")]
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
