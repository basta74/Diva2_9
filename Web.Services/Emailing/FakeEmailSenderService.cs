using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Diva2.Services.Emailing
{
    public class FakeEmailSenderService : IEmailSenderService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            return Task.FromResult(0);
        }
    }
}
