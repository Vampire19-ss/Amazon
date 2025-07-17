using System;

namespace WebApplication1.Interfaces;

public interface IMailService
{
   public Task SendEmailAsync(string emailAddress , string subject ,  string body, bool isHtml = true );
}

