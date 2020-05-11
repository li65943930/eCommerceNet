namespace eCommerceNet.Models.Requests
{
    public class ProductRequest
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public decimal Price { get; set; }

        public decimal ShippingCost { get; set; }

        public int ProductTypeId { get; set; }
    }
}
