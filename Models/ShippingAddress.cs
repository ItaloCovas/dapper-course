namespace ECommerceAPI.Models
{
    public class ShippingAddress
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string AddressName { get; set; }

        public string ZipCode { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string Street { get; set; }

        public string Number { get; set; }

        public string Complement { get; set; }

        public User User { get; set; }

    }
}