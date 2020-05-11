namespace eCommerceNet.Models.Requests
{
    public class AccountRequest
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Username { get; set; }

        public string ShippingAddress { get; set; }
    }
}
