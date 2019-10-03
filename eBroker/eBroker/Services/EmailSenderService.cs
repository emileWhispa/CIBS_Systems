using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// Decompiled with JetBrains decompiler
// Type: Forex.Services.EmailSenderService
// Assembly: eBroker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A3E47ED2-55DD-4DD2-9FD7-CF0D8F559EAC
// Assembly location: E:\source_codes\c_sharp\App\bin\eBroker.dll

using System.Net.Mail;

namespace Forex.Services
{
    public class EmailSenderService
    {
        public static void SendEmail(string body, string to, string subject)
        {
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            using (SmtpClient smtpClient = new SmtpClient())
                smtpClient.Send(message);
        }
    }
}
