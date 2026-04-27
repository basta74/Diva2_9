using Diva2.Core;
using Diva2.Core.Main.Comunications;
using Diva2.Core.Main.Users;
using Diva2.Core.Model.Json;
using Diva2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Diva2.Services.Managers.Emails
{
    public class ComunicationService : IComunicationService
    {
        private ApplicationDbContext dbContext;
        private IRepository<SmsLog> repSms;

         private string domain = "diva2.cz";
        private string domainName = "diva2.cz";
        private string smtpEmail = "info@diva2.cz";
        private string smtpPass = "h.4WJ9hfRS";


        public ComunicationService(ApplicationDbContext dbContext, IRepository<SmsLog> repSms)
        {
            this.dbContext = dbContext;
            this.repSms = repSms;
        }

        public bool SendEmail(MailMessage mail, JsonStatus resp)
        {
            bool ok = false;
            try
            {
                SmtpClient SmtpServer = this.GetSmtpClient();
                //  mail.From = new MailAddress("sys@divacal.cz");

                SmtpServer.Port = 587;
                /*
                SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password"); /**/
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                ok = true;
                resp.Status = true;

            }
            catch (Exception ex)
            {
                resp.MsgAddDanger(ex.InnerException.ToString());
            }

            return ok;
        }

        public SmtpClient GetSmtpClient()
        {
            SmtpClient smtpClient = new SmtpClient("smtp.forpsi.com")
            {
                Credentials = new System.Net.NetworkCredential(smtpEmail, smtpPass),
                EnableSsl = true
            };

            return smtpClient;
        }

        private void SetHeaders(MailMessage mail)
        {
            mail.Headers.Add("Message-Id", "<" + Guid.NewGuid().ToString() + $"@{domain}>");
            mail.From = new MailAddress(smtpEmail);
        }

        public bool SendZapomenuteHeslo(User8 user, string token ,  JsonStatus resp)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(user.Email);
                SetHeaders(mail);

                mail.Subject = "Změna hesla pro přístup do systému";


                int lng = user.Id.ToString().Length;

                Random rnd = new Random();
                int code = rnd.Next(1000000, 10000000);
                string cc = $"{lng}{code}{user.Id.ToString()}";

                string subd = (this.dbContext.SubDomain == null || this.dbContext.SubDomain == "") ? "test" : this.dbContext.SubDomain;

                string url = $"{subd}.diva2.cz/Account/updatePassword/{cc}";

                if (token.Length > 2) {

                    url = token;

                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Email byl vygenerován jako reakce na změnu hesla v systému {domainName}");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("Pokud jste tuto výzvu neprovedl(a) nereagujte na ni.");
                sb.AppendLine("");
                sb.AppendLine($"Nové heslo bude vygenerováno a zasláno po kliknití na následující odkaz {url}");
                sb.AppendLine("");
                sb.AppendLine("Email by vygenerován systémem Diva2.cz");


                mail.Body = sb.ToString();


                var isOk = SendEmail(mail, resp);

                if (isOk)
                {
                    resp.MsgAdd($"Na email {user.Email} byl odeslány informace pro obnovení hesla", JsonMessageType.Success);
                }

            }
            catch (Exception ex)
            {
                resp.MsgAdd(ex.InnerException.ToString(), JsonMessageType.Danger);
            }

          return  resp.Status;
        }

        public bool SendRegisterMessage(User8 user, string token, JsonStatus resp)
        {
            string ret = "";

            MailMessage mail = new MailMessage();
            mail.To.Add(user.Email);
            SetHeaders(mail);
            /*
            mail.Headers.Add("Message-Id", "<" + Guid.NewGuid().ToString() + "@divacal.cz>");
            mail.From = new MailAddress("sys@divacal.cz");
            /**/
            string subd = (this.dbContext.SubDomain == null || this.dbContext.SubDomain == "") ? "test" : this.dbContext.SubDomain;

            mail.Subject = $"Aktivace účtu pro v systému {subd}.diva2.cz";


            int lng = user.Id.ToString().Length;

            Random rnd = new Random();
            int code = rnd.Next(1000000, 10000000);
            string cc = $"{lng}{code}{user.Id.ToString()}";

            string url = $"{subd}.diva2.cz/Account/updatePassword/{cc}";
            if (token.Length > 2)
            {

                url = token;

            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Email byl vygenerován jako reakce Vaší registrace v systému diva2.cz");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Pokud jste tuto výzvu neprovedl(a) nereagujte na ni.");
            sb.AppendLine("");
            sb.AppendLine($"Váš účet si aktivujte kliknutím následující odkaz {url}");
            sb.AppendLine("");
            sb.AppendLine("Email by vygenerován systémem diva2.cz");


            mail.Body = sb.ToString();

            try
            {
                bool isOk = SendEmail(mail, resp);
                resp.MsgAdd($"Na email {user.Email} byl odeslány informace pro aktivaci uživatele", JsonMessageType.Success);
            }
            catch (Exception ex)
            {
                resp.MsgAdd(ex.InnerException.ToString(), JsonMessageType.Danger);
            }

            return resp.Status;
        }



        public void Insert(SmsLog smsL)
        {
            repSms.Insert(smsL);
        }

        public IPagedList<SmsLog> GetAll(int pageNumber, int pageSize)
        {
            var query = repSms.TableUntracked.OrderByDescending(d => d.Id);


            var result = new PagedList<SmsLog>(query, pageNumber, pageSize);

            return result;

           
        }

        public IPagedList<SmsLog> GetErrorsAll(int pageNumber, int pageSize)
        {
            var query = repSms.TableUntracked.Where(d => d.Vyrizeno == false).OrderByDescending(d => d.Id);
            var result = new PagedList<SmsLog>(query, pageNumber, pageSize);
            return result;

        }

        public SmsLog GetById(int id) {
            return repSms.TableUntracked.Where(d => d.Id == id).FirstOrDefault();
        }

        public void Update(SmsLog smsL)
        {
            repSms.Update(smsL);
        }

    }
}
