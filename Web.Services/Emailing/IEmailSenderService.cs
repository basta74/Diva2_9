using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Diva2.Services.Emailing
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
