using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceNet.Models;
using eCommerceNet.Models.Requests;
using eCommerceNet.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerceNet.Controllers
{
    [Route("api/shoppingCart")]
    [ApiController]
    public class ShoppingCartController : ApiBaseController
    {
        public ShoppingCartController(AppDbContext context) : base(context)
        { }

        [HttpGet]
        public async Task<OperationResult<List<ShoppingCartResponse>>> Get()
        {
            var response = new OperationResult<List<ShoppingCartResponse>>();

            try
            {
                foreach (ShoppingCart shoppingCart in await DbContext.ShoppingCarts.ToListAsync())
                {
                    response.Content.Add(new ShoppingCartResponse(shoppingCart));
                }

                response.Success = true;
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        [HttpGet("{id}/productItems")]
        public async Task<OperationResult<List<ProductItemResponse>>> GetProducts(int id)
        {
            var response = new OperationResult<List<ProductItemResponse>>();

            try
            {
                var messages = ValidateRequest(id);
                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                var dbShoppingCart = await DbContext.ShoppingCarts
                                                    .Include(s => s.ProductItems)
                                                    .FirstOrDefaultAsync(c => c.Id == id);

                foreach (ProductItem productItem in dbShoppingCart.ProductItems)
                {
                    response.Content.Add(new ProductItemResponse(productItem));
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
        public async Task<OperationResult<ShoppingCartResponse>> Get(int id)
        {
            var response = new OperationResult<ShoppingCartResponse>();

            try
            {
                var messages = ValidateRequest(id);
                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                var dbShoppingCart = await DbContext.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == id);

                response.Success = true;
                response.Content = new ShoppingCartResponse(dbShoppingCart);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        [HttpPut("{id}")]
        public async Task<OperationResult<ShoppingCartResponse>> Put(int id, [FromBody] ShoppingCartRequest request)
        {
            var response = new OperationResult<ShoppingCartResponse>();

            try
            {
                var messages = ValidateRequest(id);
                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                var dbShoppingCart = await UpdateShoppingCart(id, request);

                await DbContext.SaveChangesAsync();

                response.Success = true;
                response.Content = new ShoppingCartResponse(dbShoppingCart);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        #region Auxiliary methods

        protected override List<string> ValidateRequest(int id)
        {
            var messages = base.ValidateRequest(id);
            if (messages.Count > 0)
            {
                return messages;
            }

            if (!DbContext.ShoppingCarts.Any(c => c.Id == id))
            {
                messages.Add("Id: shopping cart id does not exist");
            }

            return messages;
        }

        private async Task<ShoppingCart> UpdateShoppingCart(int id, ShoppingCartRequest request)
        {
            var shoppingCart = await DbContext.ShoppingCarts.FirstOrDefaultAsync(s => s.Id.Equals(id));
            shoppingCart.Purchased = request.Purchased;

            return shoppingCart;
        }

        #endregion
    }
}