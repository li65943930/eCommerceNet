namespace eCommerceNet.Models.Responses
{
    public class ProductResponse
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public decimal Price { get; set; }

        public decimal ShippingCost { get; set; }

        public ProductResponse() 
        { }

        public ProductResponse(Product product)
        {
            Id = product.Id;
            Description = product.Description;
            ImagePath = product.ImagePath;
            Price = product.Price;
            ShippingCost = product.ShippingCost; 

        }
    }
}
