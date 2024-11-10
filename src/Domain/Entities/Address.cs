using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("addresses")]
[Index("IdOrder", Name = "fk_orders_id_addresses_id_order_idx")]
public class Address
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("id_order")]
    [Required]
    public int IdOrder { get; set; }

    [Column("street_name")]
    [Required]
    [StringLength(64)]
    public string StreetName { get; set; }

    [Column("street_number")]
    [Required]
    [StringLength(8)]
    public string StreetNumber { get; set; }

    [Column("province")]
    [Required]
    [StringLength(20)]
    public string Province { get; set; }

    [Column("locality")]
    [Required]
    [StringLength(64)]
    public string Locality { get; set; }

    [ForeignKey("IdOrder")]
    [InverseProperty("IdAddressNavigation")]
    public virtual Order IdOrderNavigation { get; set; }
}
