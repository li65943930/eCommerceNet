using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using eCommerceNet.Models;
using eCommerceNet.Models.Responses;
using Microsoft.EntityFrameworkCore;
using eCommerceNet.Models.Requests;

namespace eCommerceNet.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class    ProductController : ApiBaseController
    {
        public ProductController(AppDbContext context) : base(context)
        { }

        [HttpGet]
        public async Task<OperationResult<List<ProductResponse>>> Get()
        {
            var response = new OperationResult<List<ProductResponse>>();

            try
            {
                foreach (Product product in await DbContext.Products.ToListAsync())
                {
                    response.Content.Add(new ProductResponse(product));
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
        public async Task<OperationResult<ProductResponse>> Get(int id)
        {
            var response = new OperationResult<ProductResponse>();

            try 
            {
                var messages = ValidateRequest(id);
                messages.AddRange(ProductExists(id));

                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                var dbProduct = await DbContext.Products.FirstOrDefaultAsync(c => c.Id == id);
                response.Success = true;
                response.Content = new ProductResponse(dbProduct);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        [HttpGet("{id}/comments")]
        public async Task<OperationResult<List<CommentResponse>>> GetComments(int id)
        {
            var response = new OperationResult<List<CommentResponse>>();

            try
            {
                var messages = ValidateRequest(id);
                messages.AddRange(ProductExists(id));

                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                var dbProduct = await DbContext.Products
                                               .Include(s => s.Comments)
                                               .FirstOrDefaultAsync(c => c.Id == id);
                
                foreach (Comment comment in dbProduct.Comments)
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

        [HttpPost]
        public async Task<OperationResult<ProductResponse>> Post([FromBody] ProductRequest request)
        {
            var response = new OperationResult<ProductResponse>();

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

                var result = await DbContext.Products.AddAsync(new Product(request));
                await DbContext.SaveChangesAsync();
                response.Success = true;
                response.Content = new ProductResponse(result.Entity);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        [HttpPut("{id}")]
        public async Task<OperationResult<ProductResponse>> Put(int id, [FromBody] ProductRequest request)
        {
            var response = new OperationResult<ProductResponse>();

            try
            {
                var messages = ValidateRequest(id);
                messages.AddRange(ValidateRequest(request));
                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                var dbProduct = await UpdateProduct(id, request);
                await DbContext.SaveChangesAsync();

                response.Success = true;
                response.Content = new ProductResponse(dbProduct);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public OperationResult<ProductResponse> Delete(int id)
        {
            var response = new OperationResult<ProductResponse>();

            try
            {
                var messages = ValidateRequest(id);
                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    response.Success = false;
                    return response;
                }

                var dbProduct = DbContext.Products.FirstOrDefault(c => c.Id == id);
                DbContext.Products.Remove(dbProduct);
                DbContext.SaveChangesAsync();

                response.Success = true;
                response.Content = new ProductResponse(dbProduct);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response; 
        }

        #region Auxiliary methods

        public List<string> ProductExists(int id)
        {
            var messages = new List<string>();

            if (!DbContext.Products.Any(p => p.Id == id))
            {
                messages.Add("Id: product does not exist");
            }

            return messages;
        }

        private async Task<Product> UpdateProduct(int id, ProductRequest request)
        {
            var dbProduct = await DbContext.Products.FirstOrDefaultAsync(s => s.Id.Equals(id));

            dbProduct.Description = request.Description;
            dbProduct.ImagePath = request.ImagePath;
            dbProduct.Price = request.Price;
            dbProduct.ShippingCost = request.ShippingCost;

            return dbProduct;
        }

        private List<string> ValidateRequest(ProductRequest request)
        {
            var messages = new List<string>();

            if (request.Id < 0)
            {
                messages.Add("Id: invalid (must be an integer greater than or equals to 0)");
            }

            if (request.Price <= 0 )
            {
                messages.Add("Price: invalid (must be a valid decimal greater than 0)");
            }
            
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                messages.Add("Description: product description cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(request.ImagePath)) 
            {
                messages.Add("ImagePath: product image cannot be empty");
            }

            if (request.ShippingCost < 0) 
            {
                messages.Add("ShippingCost: invalid (must be a valid decimal greater than or equals 0)");
            }

            return messages;
        }

        private List<string> ValidateDuplicate(ProductRequest request)
        {
            var messages = new List<string>();

            var isDuplicate = DbContext.Products.Any(p => p.Id ==  request.Id);

            if (isDuplicate)
            {
                messages.Add("Id: invalid (product already exists)");
            }

            return messages;
        }

        #endregion
    }
}
