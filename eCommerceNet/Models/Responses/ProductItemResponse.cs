namespace eCommerceNet.Models.Responses
{
    public class ProductItemResponse
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int ProductId { get; set; }

        public ProductItemResponse()
        { }

        public ProductItemResponse(ProductItem productItem)
        {
            Id = productItem.Id;
            Description = productItem.Description;
            Price = productItem.Price;
            Quantity = productItem.Quantity;
            ProductId = productItem.ProductId;
        }
    }
}
