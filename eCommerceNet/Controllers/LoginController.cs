using System.Linq;
using System.Threading.Tasks;
using eCommerceNet.Utilities;
using eCommerceNet.Models;
using eCommerceNet.Models.Requests;
using eCommerceNet.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceNet.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ApiBaseController
    {
        public LoginController(AppDbContext context) : base(context)
        {
        }

        [HttpPost]
        public async Task<OperationResult<LoginResponse>> Post([FromBody] LoginRequest request)
        {
            var response = new OperationResult<LoginResponse>();

            try
            {
                var user = DbContext.Accounts.FirstOrDefault(u => u.Username == request.Username);
                if (user != null)
                {
                    if (PasswordHash.VerifyHashedPassword(user.Password, request.Password))
                    {
                        await HttpContext.Session.LoadAsync();
                        HttpContext.Session.SetInt32("AccountId", user.Id);
                        await HttpContext.Session.CommitAsync();

                        response.Success = true;
                        response.Content.Username = user.Username;
                    }
                }
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        [HttpDelete]
        public async Task<OperationResult<NoContentResponse>> Delete()
        {
            var response = new OperationResult<NoContentResponse>();

            try
            {
                await HttpContext.Session.LoadAsync();
                HttpContext.Session.SetInt32("AccountId", 0);
                await HttpContext.Session.CommitAsync();
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            response.Success = true;

            return response;
        }
    }
}