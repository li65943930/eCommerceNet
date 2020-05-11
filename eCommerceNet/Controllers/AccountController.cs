using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using eCommerceNet.Utilities;
using eCommerceNet.Models;
using eCommerceNet.Models.Requests;
using eCommerceNet.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace eCommerceNet.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ApiBaseController
    {
        public AccountController(AppDbContext context) : base(context)
        { }

        // GET: api/account/username
        [HttpGet("{username}")]
        public async Task<OperationResult<AccountResponse>> Get(string username)
        {
            var response = new OperationResult<AccountResponse>();

            try
            {
                var result = await ValidateUser(username);
                if(!string.IsNullOrEmpty(result.Item1))
                {
                    response.Messages.Add(result.Item1);
                    return response;
                }

                response.Success = true;
                response.Content = new AccountResponse(result.Item2);
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        // POST: api/account
        [HttpPost]
        public async Task<OperationResult<NoContentResponse>> Post([FromBody] AccountRequest request)
        {
            var response = new OperationResult<NoContentResponse>();

            try
            {
                var messages = ValidateRequest(request);

                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    return response;
                }

                Account account = new Account()
                {
                    Email = request.Email,
                    Password = PasswordHash.HashPassword(request.Password),
                    Username = request.Username,
                    ShippingAddress = request.ShippingAddress
                };

                var dbAccount = await DbContext.Accounts.AddAsync(account);
                await DbContext.SaveChangesAsync();
                response.Success = true;
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        // PUT: api/account/username
        [HttpPut("{username}")]
        public async Task<OperationResult<NoContentResponse>> Put(string username, [FromBody] AccountRequest request)
        {
            var response = new OperationResult<NoContentResponse>();

            try
            {
                var result = await ValidateUser(username);
                if (!string.IsNullOrEmpty(result.Item1))
                {
                    response.Messages.Add(result.Item1);
                    return response;
                }

                var messages = new List<string>();
                messages.AddRange(ValidateRequest(request));
                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    return response;
                }

                var dbAccount = await UpdateAccount(AccountId, request);
                await DbContext.SaveChangesAsync();

                response.Success = true;
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        // Delete currently logged in user
        //DELETE api/account/
        [HttpDelete]
        public async Task<OperationResult<NoContentResponse>> Delete()
        {
            var response = new OperationResult<NoContentResponse>();

            try
            {
                var messages = ValidateRequest(AccountId);
                if (messages.Count > 0)
                {
                    response.Messages = messages;
                    return response;
                }

                var dbAccount = DbContext.Accounts.FirstOrDefault(c => c.Id == AccountId);
                DbContext.Accounts.Remove(dbAccount);
                await DbContext.SaveChangesAsync();

                response.Success = true;
            }
            catch
            {
                response.Messages.Add("InternalError");
            }

            return response;
        }

        #region Auxiliary methods

        private async Task<(string, Account)> ValidateUser(string username)
        {
            string message = string.Empty;
            var dbAccount = await DbContext.Accounts.FirstOrDefaultAsync(a => a.Username == username);

            if (dbAccount == null)
            {
                message = "WrongRequest";
                return (message, dbAccount);
            }

            var loggedInAccount = await DbContext.Accounts.FirstOrDefaultAsync(a => a.Id == AccountId);
            if (loggedInAccount == null || dbAccount.Id != loggedInAccount.Id)
            {
                message = "AccessDenied";
                return (message, dbAccount);
            }

            return (message, dbAccount);
        }

        private async Task<Account> UpdateAccount(int id, AccountRequest request)
        {
            var dbAccount = await DbContext.Accounts.FirstOrDefaultAsync(a => a.Id.Equals(id));
            dbAccount.Email = request.Email;
            dbAccount.Password = PasswordHash.HashPassword(request.Password);
            dbAccount.Username = request.Username;
            dbAccount.ShippingAddress = request.ShippingAddress;

            return dbAccount;
        }

        private List<string> ValidateRequest(AccountRequest request)
        {
            var messages = new List<string>();

            if (!IsValidEmail(request.Email))
            {
                messages.Add("EmailNotValid");
            }

            if (!IsValidPassword(request.Password))
            {
                messages.Add("PasswordNotValid");
            }

            if (!isValidUsername(request.Username))
            {
                messages.Add("UsernameNotValid");
            }

            if (string.IsNullOrEmpty(request.ShippingAddress))
            {
                messages.Add("ShippingAddressEmpty");
            }

            messages.AddRange(ValidateDuplicate(request));

            return messages;
        }

        private List<string> ValidateDuplicate(AccountRequest request)
        {
            var messages = new List<string>();

            if (DbContext.Accounts.Any(u => u.Email == request.Email))
            {
                messages.Add("EmailRepeated");
            }

            if (DbContext.Accounts.Any(u => u.Username == request.Username))
            {
                messages.Add("UsernameRepeated");
            }

            return messages;
        }

        private bool IsValidEmail(string target)
        {
            var emailValidator = new EmailAddressAttribute();
            return emailValidator.IsValid(target);
        }

        private bool IsValidPassword(string target)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasSpecialCharacter = new Regex(@"[\\!#$%&'()*+,-./:;<=>?@[\]^_`{|}~]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return hasNumber.IsMatch(target) && hasUpperChar.IsMatch(target) && hasSpecialCharacter.IsMatch(target) && hasMinimum8Chars.IsMatch(target);
        }

        private bool isValidUsername(string target)
        {
            var hasMinimum5Chars = new Regex(@".{5,}");

            return hasMinimum5Chars.IsMatch(target);
        }

        #endregion
    }
}
