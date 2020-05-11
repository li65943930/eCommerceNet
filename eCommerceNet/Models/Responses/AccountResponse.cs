using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceNet.Models.Responses
{
    public class AccountResponse
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string ShippingAddress { get; set; }

        public AccountResponse() { }

        public AccountResponse(Account account)
        {
            Email = account.Email;
            Username = account.Username;
            ShippingAddress = account.ShippingAddress;
        }
    }
}
