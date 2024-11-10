using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Client : User
    {
        public Client()
        {
            Role = Role.Client;
        }

        [InverseProperty("IdUserNavigation")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
