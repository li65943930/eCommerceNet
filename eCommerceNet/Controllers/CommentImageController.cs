using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eCommerceNet.Models;
using eCommerceNet.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceNet.Controllers
{
    [Route("api/commentImage")]
    [ApiController]
    public class CommentImageController : ApiBaseController
    {
        public CommentImageController(AppDbContext context) : base(context)
        { }

        [HttpPost("{id}")]
        public async Task<OperationResult<CommentImageUploadResponse>> Post(IFormFile file, int id)
        {
            var response = new OperationResult<CommentImageUploadResponse>();
            
            if (IsLoggedIn)
            {
                try
                {
                    var messages = ValidateRequest(id);
                    messages.AddRange(ValidateFile(file));
                    messages.AddRange(IsCommentExisted(id));

                    if (messages.Count > 0)
                    {
                        response.Messages = messages;
                        response.Success = false;
                        return response;
                    }

                    var filePath = Path.Combine("wwwroot\\data", Path.GetRandomFileName());
                    CommentImage commentImage = new CommentImage();
                    commentImage.CommentId = id;
                    commentImage.ImagePath = filePath;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        response.Success = true;
                        var result = await DbContext.CommentImages.AddAsync(commentImage);
                        await DbContext.SaveChangesAsync();
                    }

                    response.Content = new CommentImageUploadResponse()
                    {
                        CommentId = commentImage.CommentId,
                        FilePath = commentImage.ImagePath
                    };
                }
                catch
                {
                    response.Messages.Add("InternalError");
                }
            }

            return response;
        }

        public List<string> ValidateFile(IFormFile file)
        {
            var messages = new List<string>();

            if (file.Length < 0)
            {
                messages.Add("File: invalid ( file length must greater than 0)");
            }

            return messages;
        }

        public List<string> IsCommentExisted(int id)
        {
            var messages = new List<string>();

            if (!DbContext.Comments.Any(c => c.Id == id))
            {
                messages.Add("CommentId: invalid (comment is not exist)");
            }
            
            return messages;
        }
    }
}
