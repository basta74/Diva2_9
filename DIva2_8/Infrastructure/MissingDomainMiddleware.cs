using Diva2.Data.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Infrastructure
{
    public class MissingDomainMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string  missingDomainUrl;

        public MissingDomainMiddleware(RequestDelegate next, string missingDomainUrl)
        {
            this.next = next;
            this.missingDomainUrl = missingDomainUrl;
        }

        public async Task Invoke(HttpContext httpContext, IDomainService domainService)
        {
            if (domainService.Domain == null)
            {
                httpContext.Response.Redirect(missingDomainUrl);
                return;
            }

            await next.Invoke(httpContext);
        }
    }
}
