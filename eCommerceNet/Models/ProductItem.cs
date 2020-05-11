using eCommerceNet.Models.Requests;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerceNet.Models
{
    public class ProductItem
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey(nameof(ShoppingCart))]
        public int ShoppingCartId { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }
    }
}
