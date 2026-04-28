using Diva2.Core.Main.Users;
using Diva2.Data.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Diva2Web.Areas.Admin
{

    public class AuthorizeActionFilterAttribute : TypeFilterAttribute
    {
        public AuthorizeActionFilterAttribute() : base(typeof(AuthorizeActionFilter))
        {

        }

        public class AuthorizeActionFilter : IAuthorizationFilter
        {
            private readonly IDomainService domainService;

            public AuthorizeActionFilter(IDomainService domainService)
            {
                this.domainService = domainService;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var session = context.HttpContext.Session;
                var domain = domainService.Domain.name;
                string key = $"{domain}-userId";
                bool nameOK = session.TryGetValue(key, out var sessionName);

                if (!nameOK)
                {
                    context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
                }
                /**/
            }
        }
    }

}
