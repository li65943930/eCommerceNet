using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceNet.Models.Responses
{
    public class CommentImageUploadResponse
    {

        public string FilePath { get; set; }

        public int CommentId { get; set; }

        public CommentImageUploadResponse()
        { }
    }
}
