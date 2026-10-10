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

        public MissingDomainMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext, IDomainService domainService)
        {
            if (domainService.Domain == null)
            {
                // neznámá subdoména: tichá 404, nic nelogovat a nesahat do DB
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            await next.Invoke(httpContext);
        }
    }
}
