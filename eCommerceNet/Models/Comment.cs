using eCommerceNet.Models.Requests;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerceNet.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int Rating { get; set; }
        
        public string Text { get; set; }

        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public virtual ICollection<CommentImage> CommentImages { get; set; }

        public Comment()
        {
            CommentImages = new HashSet<CommentImage>();
        }

        public Comment(CommentRequest request) : this()
        {
            Rating = request.Rating;
            Text = request.Text;
            AccountId = request.AccountId;
            ProductId = request.ProductId;
        }
    }
}