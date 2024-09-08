using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("categories")]
[Index("IdParent", Name = "fk_categories_id_categories_id_parent_idx")]
[Index("Name", Name = "name_UNIQUE", IsUnique = true)]
public class Category
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("name")]
    [Required]
    [StringLength(32)]
    public string Name { get; set; }

    [Column("id_parent")]
    public int? IdParent { get; set; }

    [ForeignKey("IdParent")]
    [InverseProperty("InverseIdParentNavigation")]
    public virtual Category? IdParentNavigation { get; set; }

    [InverseProperty("IdParentNavigation")]
    public virtual ICollection<Category> InverseIdParentNavigation { get; set; } = new List<Category>();

    [InverseProperty("IdCategoryNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
