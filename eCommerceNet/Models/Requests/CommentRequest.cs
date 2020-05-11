namespace eCommerceNet.Models.Requests
{
    public class CommentRequest
    {
        public int Rating { get; set; }

        public string Text { get; set; }

        public int AccountId { get; set; }

        public int ProductId { get; set; }
    }
}
