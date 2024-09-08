using Domain.Enums;

namespace Domain.Entities
{
    public class Sysadmin : User
    {
        public Sysadmin()
        {
            Role = Role.Sysadmin;
        }
    }
}
