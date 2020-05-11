using System.Collections.Generic;
using eCommerceNet.Models.Requests;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerceNet.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Description { get; set; }
        
        public string ImagePath { get; set; }
        
        public decimal Price { get; set; }
        
        public decimal ShippingCost { get; set; }

        [ForeignKey(nameof(ProductType))]
        public int ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public Product()
        {
            Comments = new HashSet<Comment>();
        }

        public Product(ProductRequest request) : this()
        {
            Id = request.Id;
            Description = request.Description;
            Price = request.Price;
            ImagePath = request.ImagePath;
            ShippingCost = request.ShippingCost;
            ProductTypeId = request.ProductTypeId;
        }
    }
}
