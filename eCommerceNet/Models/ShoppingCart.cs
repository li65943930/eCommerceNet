using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerceNet.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public bool Purchased { get; set; }

        [ForeignKey(nameof(Account))]
        public int? AccountId { get; set; }
        public virtual Account Account { get; set; }

        public virtual ICollection<ProductItem> ProductItems { get; set; }

        public ShoppingCart()
        {
            ProductItems = new HashSet<ProductItem>();
        }
    }
}
