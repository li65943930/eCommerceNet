using System.Collections.Generic;

namespace eCommerceNet.Models.Responses
{
    public class OperationResult<T> where T : new()
    {
        public bool Success { get; set; }

        public List<string> Messages { get; set; }

        public T Content { get; set; }

        public OperationResult()
        {
            Messages = new List<string>();
            Content = new T();
        }
    }
}