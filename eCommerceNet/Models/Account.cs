using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eCommerceNet.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }

        public Account()
        {
            Comments = new HashSet<Comment>();
            ShoppingCarts = new HashSet<ShoppingCart>();
        }
    }
}