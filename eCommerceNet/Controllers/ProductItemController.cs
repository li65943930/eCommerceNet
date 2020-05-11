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
    [Route("api/productItem")]
    [ApiController]
    public class ProductItemController : ApiBaseController
    {
        const decimal TAX_RATE = 0.13m;

        public ProductItemController(AppDbContext context) : base(context)
        { }

        [HttpGet]
        public async Task<OperationResult<List<ProductItemResponse>>> Get()
        {
            var response = new OperationResult<List<ProductItemResponse>>();

            try
            {
                foreach (ProductItem productItem in await DbContext.ProductItems.ToListAsync())
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
        public async Task<OperationResult<ProductItemResponse>> Get(int id)
        {
            var response = new OperationResult<ProductItemResponse>();

            try
            {
                var messages = ValidateRequest(id);
                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                var dbComment = await DbContext.ProductItems.FirstOrDefaultAsync(c => c.Id == id);

                response.Success = true;
                response.Content = new ProductItemResponse(dbComment);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        [HttpPost]
        public async Task<OperationResult<ProductItemResponse>> Post([FromBody] ProductItemRequest request)
        {
            var response = new OperationResult<ProductItemResponse>();

            try
            {
                var messages = ValidateRequest(request);
                messages.AddRange(ValidateDuplicate(request));
                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                await UpdateShoppingCartId();
                var productItem = await GetProductItem(request);

                var result = await DbContext.ProductItems.AddAsync(productItem);
                await DbContext.SaveChangesAsync();
                response.Success = true;
                response.Content = new ProductItemResponse(result.Entity);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        [HttpPut("{id}")]
        public async Task<OperationResult<ProductItemResponse>> Put(int id, [FromBody] ProductItemRequest request)
        {
            var response = new OperationResult<ProductItemResponse>();

            try
            {
                var messages = ValidateRequest(id);
                messages.AddRange(ValidateQuantity(request));
                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                var dbComment = await UpdateProductItem(id, request);
                await DbContext.SaveChangesAsync();

                response.Success = true;
                response.Content = new ProductItemResponse(dbComment);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;    
        }

        [HttpDelete("{id}")]
        public async Task<OperationResult<ProductItemResponse>> Delete(int id)
        {
            var response = new OperationResult<ProductItemResponse>();

            try
            {
                var messages = ValidateRequest(id);
                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                var dbProductItem = await DbContext.ProductItems.FirstOrDefaultAsync(c => c.Id == id);
                
                DbContext.ProductItems.Remove(dbProductItem);
                await DbContext.SaveChangesAsync();

                response.Success = true;
                response.Content = new ProductItemResponse(dbProductItem);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        #region Auxiliary methods

        private async Task<ProductItem> GetProductItem(ProductItemRequest request)
        {
            var product = await DbContext.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId);

            return new ProductItem
            {
                Price = await GetTotalPriceForItem(product),
                Description = product.Description,
                ProductId = product.Id,
                ShoppingCartId = ShoppingCartId,
                Quantity = request.Quantity
            };
        }

        private async Task UpdateShoppingCartId()
        {
            if (ShoppingCartId <= 0)
            {
                ShoppingCart shoppingCart = new ShoppingCart
                {
                    AccountId = AccountId > 0 ? AccountId : (int?) null
                };

                var dbShoppingCart = await DbContext.ShoppingCarts.AddAsync(shoppingCart);

                await DbContext.SaveChangesAsync();

                ShoppingCartId = dbShoppingCart.Entity.Id;
            }
        }

        private async Task<ProductItem> UpdateProductItem(int id, ProductItemRequest request)
        {
            var dbProductItem = await DbContext.ProductItems.FirstOrDefaultAsync(s => s.Id.Equals(id));
            dbProductItem.Quantity = request.Quantity;
            
            return dbProductItem;
        }

        private List<string> ValidateRequest(ProductItemRequest request)
        {
            var messages = new List<string>();

            if (request.ProductId <= 0)
            {
                messages.Add("ProductId: invalid (must be an integer greater than or equals to 0)");
            }
            else if (!DbContext.Products.Any(p => p.Id == request.ProductId))
            {
                messages.Add("ProductId: product id does not exist");
            }

            messages.AddRange(ValidateQuantity(request));

            if (request.Quantity < 1 || request.Quantity > 100)
            {
                messages.Add("Quantity: invalid (must be an integer between 1 and 100)");
            }

            return messages;
        }

        private List<string> ValidateQuantity(ProductItemRequest request)
        {
            var messages = new List<string>();

            if (request.Quantity < 1 || request.Quantity > 100)
            {
                messages.Add("Quantity: invalid (must be an integer between 1 and 100)");
            }

            return messages;
        }

        private List<string> ValidateDuplicate(ProductItemRequest request)
        {
            var messages = new List<string>();

            var isDuplicate = DbContext.ProductItems.Any(
                p => p.ShoppingCartId == ShoppingCartId
                && p.ProductId == request.ProductId);

            if (isDuplicate)
            {
                messages.Add("ProductId: invalid (product id already exists)");
            }

            return messages;
        }

        private async Task<decimal> GetTotalPriceForItem(Product product)
        {
            var productType = await DbContext.ProductTypes.FirstOrDefaultAsync(p => p.Id == product.ProductTypeId);
            var isTaxable = productType.IsTaxable;

            var price = product.Price;
            price += product.ShippingCost;

            if (isTaxable)
            {
                price *= 1 + TAX_RATE;
            }

            return price;
        }

        protected override List<string> ValidateRequest(int id)
        {
            var messages = base.ValidateRequest(id);
            if (messages.Count > 0)
            {
                return messages;
            }

            if (!DbContext.ProductItems.Any(c => c.Id == id))
            {
                messages.Add("Id: product item id does not exist");
            }

            return messages;
        }

        #endregion
    }
}