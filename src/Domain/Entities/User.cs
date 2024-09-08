using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("users")]
[Index("Email", Name = "email_UNIQUE", IsUnique = true)]
[Index("Role", Name = "role_idx")]
public abstract class User
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("role")]
    public Role Role { get; set; }

    [Column("first_name")]
    [Required]
    [StringLength(64)]
    public string FirstName { get; set; }

    [Column("last_name")]
    [Required]
    [StringLength(64)]
    public string LastName { get; set; }

    [Column("email")]
    [Required]
    [StringLength(128)]
    public string Email { get; set; }

    [Column("password")]
    [Required]
    [StringLength(255)]
    public string Password { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime UpdatedAt { get; set; }
}
