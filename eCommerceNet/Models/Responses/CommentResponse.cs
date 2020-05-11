namespace eCommerceNet.Models.Responses
{
    public class CommentResponse
    {
        public int Id { get; set; }

        public int Rating { get; set; }

        public string Text { get; set; }

        public int AccountId { get; set; }

        public int ProductId { get; set; }

        public CommentResponse()
        { }

        public CommentResponse(Comment comment)
        {
            Id = comment.Id;
            Rating = comment.Rating;
            Text = comment.Text;
            AccountId = comment.AccountId;
            ProductId = comment.ProductId;
        }
    }
}
