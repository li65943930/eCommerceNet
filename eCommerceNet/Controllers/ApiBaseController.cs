using eCommerceNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace eCommerceNet.Controllers
{
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected AppDbContext DbContext { get; private set; }

        public ApiBaseController(AppDbContext context)
        {
            this.DbContext = context;
        }

        public bool IsLoggedIn
        {
            get
            {
                if (HttpContext.Session.GetInt32("AccountId") != 0)
                {
                    return true;
                }

                return false;
            }
        }

        public int ShoppingCartId
        {
            get
            {
                return HttpContext.Session.GetInt32("ShoppingCartId") ?? 0;
            }
            set
            {
                HttpContext.Session.SetInt32("ShoppingCartId", value);
            }
        }

        public int AccountId
        {
            get
            {
                return HttpContext.Session.GetInt32("AccountId") ?? 0;
            }
            set
            {
                HttpContext.Session.SetInt32("AccountId", value);
            }
        }

        protected virtual List<string> ValidateRequest(int id)
        {
            var messages = new List<string>();

            if (id <= 0)
            {
                messages.Add("Id: invalid (must be an integer greater than or equals to 0)");
            }

            return messages;
        }
    }
 }