using Diva2.Core;
using Diva2.Core.Main.Comunications;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Diva2.Services.Managers.Emails
{
    public interface IComunicationService
    {
        bool  SendEmail(MailMessage msg, Core.Model.Json.JsonStatus resp);

        bool SendZapomenuteHeslo(User8 user, string token, Core.Model.Json.JsonStatus resp);

        bool SendRegisterMessage(User8 user, string v, Core.Model.Json.JsonStatus resp);
        
        void Insert(SmsLog smsL);

        void Update(SmsLog smsL);

        public SmsLog GetById(int id);

        IPagedList<SmsLog> GetAll(int pageNumber, int pageSize);

        IPagedList<SmsLog> GetErrorsAll(int pageNumber, int pageSize0);

    }
}
