namespace Application.Models.Address
{
    public class ReadAddressDto
    {
        public int Id { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string Province { get; set; }
        public string Locality { get; set; }
    }
}
