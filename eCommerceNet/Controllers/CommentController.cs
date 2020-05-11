using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceNet.Models;
using eCommerceNet.Models.Requests;
using eCommerceNet.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerceNet.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ApiBaseController
    {
        public CommentController(AppDbContext context) : base(context)
        {}

        [HttpGet]
        public async Task<OperationResult<List<CommentResponse>>> Get()
        {
            var response = new OperationResult<List<CommentResponse>>();

            try
            {
                foreach (Comment comment in await DbContext.Comments.ToListAsync())
                {
                    response.Content.Add(new CommentResponse(comment));
                }

                response.Success = true;
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        [HttpGet("{id}")]
        public async Task<OperationResult<CommentResponse>> Get(int id)
        {
            var response = new OperationResult<CommentResponse>();

            try
            {
                var messages= ValidateRequest(id);
                messages.AddRange(IsCommentExisted(id));

                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                var dbComment = await DbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);

                response.Success = true;
                response.Content = new CommentResponse(dbComment);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        [HttpPost]
        public async Task<OperationResult<CommentResponse>> Post([FromBody] CommentRequest request)
        {
            var response = new OperationResult<CommentResponse>();
            if (IsLoggedIn)
            {
                try
                {
                    request.AccountId = HttpContext.Session.GetInt32("AccountId") ?? 0;

                    var messages = ValidateRequest(request);
                    if (messages.Count > 0)
                    {
                        response.Messages = messages;
                        response.Success = false;
                        return response;
                    }

                    var result = await DbContext.Comments.AddAsync(new Comment(request));
                    await DbContext.SaveChangesAsync();
                    response.Success = true;
                    response.Content = new CommentResponse(result.Entity);
                }
                catch
                {
                    response.Messages.Add("InternalError");
                }
            }
            else
            {
                response.Success = false;
            }

            return response;
        }

        [HttpPut("{id}")]
        public async Task<OperationResult<CommentResponse>> Put(int id, [FromBody] CommentRequest request)
        {
            var response = new OperationResult<CommentResponse>();
            if (IsLoggedIn)
            {
                try
                {
                    var messages = ValidateRequest(request);
                    messages.AddRange(ValidateRequest(id));
                    messages.AddRange(IsCommentExisted(id));

                    if (messages.Count > 0)
                    {
                        response.Messages = messages;
                        response.Success = false;
                        return response;
                    }

                    var dbComment = DbContext.Comments.FirstOrDefault(s => s.Id.Equals(id));
                    dbComment.Rating = request.Rating;
                    dbComment.Text = request.Text;
                    await DbContext.SaveChangesAsync();

                    response.Success = true;
                    response.Content = new CommentResponse(dbComment);
                }
                catch
                {
                    response.Messages.Add("InternalError");
                }
            }
            else
            {
                response.Success = false;
            }

            return response;

        }

        [HttpDelete("{id}")]
        public OperationResult<CommentResponse> Delete(int id)
        {
            var response = new OperationResult<CommentResponse>();
            
            if (IsLoggedIn)
            {
                try
                {
                    var messages = ValidateRequest(id);
                    messages.AddRange(IsCommentExisted(id));

                    if (messages.Count > 0)
                    {
                        response.Messages = messages;
                        response.Success = false;
                        return response;
                    }

                    var dbComment = DbContext.Comments.FirstOrDefault(c => c.Id == id);
                    DbContext.Comments.Remove(dbComment);
                    DbContext.SaveChangesAsync();

                    response.Success = true;
                    response.Content = new CommentResponse(dbComment);
                }
                catch
                {
                    response.Messages.Add("InternalError");
                }
            }
            else
            {
                response.Success = false;
            }

            return response;
        }

        public List<string> ValidateRequest(CommentRequest request)
        {
            var messages = new List<string>();

            if (request.Rating < 1 || request.Rating >5)
            {
                messages.Add("Rate: invalid (must be an integer between 1 and 5.)");
            }

            if (string.IsNullOrEmpty(request.Text) && request.Text.Length <= 0)
            {
                messages.Add("Text: invalid (at least one character.)");
            }

            if (request.ProductId < 0)
            {
                messages.Add("ProductId: invalid (must be an integer greater than or equal to 0.)");
            }

            if (!DbContext.ProductItems.Any(p => p.ShoppingCart.AccountId == request.AccountId
                                            && p.ShoppingCart.Purchased
                                            && p.ProductId == request.ProductId))
            {
                messages.Add("AccountId: invalid (must have bought this product.)");
            }

            return messages;
        }

        public List<string> IsCommentExisted(int id)
        {
            var messages = new List<string>();

            if (!DbContext.Comments.Any(i => i.Id == id))
            {
                messages.Add("Id: invalid (comment does not exist)");
            }

            return messages;
        }

    }
}