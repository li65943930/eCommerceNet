namespace eCommerceNet.Models.Responses
{
    public class ShoppingCartResponse
    {
        public int Id { get; set; }

        public bool Purchased { get; set; }

        public int? AccountId { get; set; }

        public ShoppingCartResponse()
        { }

        public ShoppingCartResponse(ShoppingCart shoppingCart)
        {
            Id = shoppingCart.Id;
            Purchased = shoppingCart.Purchased;
            AccountId = shoppingCart.AccountId;
        }
    }
}