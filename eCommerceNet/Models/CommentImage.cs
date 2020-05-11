using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerceNet.Models
{
    public class CommentImage
    {
        public int Id { get; set; }

        public string ImagePath { get; set; }

        [ForeignKey(nameof(Comment))]
        public int CommentId { get; set; }
        public virtual Comment Comment { get; set; }
    }
}
