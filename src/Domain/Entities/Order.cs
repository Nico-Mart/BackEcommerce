using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("orders")]
[Index("IdUser", Name = "fk_users_id_orders_id_user_idx")]
public class Order
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("id_user")]
    [Required]
    public int IdUser { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("IdUser")]
    [InverseProperty("Orders")]
    public virtual Client IdUserNavigation { get; set; }

    [InverseProperty("IdOrderNavigation")]
    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    [InverseProperty("IdOrderNavigation")]
    public virtual Address IdAddressNavigation { get; set; }
}
