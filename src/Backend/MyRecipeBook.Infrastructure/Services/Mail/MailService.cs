using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Cryptography;
using MyRecipeBook.Domain.Services.Mail;
using MyRecipeBook.Domain.Services.Mail.Models;
using MyRecipeBook.Domain.Services.Mail.Schema;
using MyRecipeBook.Infrastructure.Services.Mail.Helper;

namespace MyRecipeBook.Infrastructure.Services.Mail;

public class MailService : IMailService
{
    private readonly MailSetting _mailSetting;

    public MailService(IOptions<MailSetting> mailSetting)
    {
        _mailSetting = mailSetting.Value;
    }

    public  async Task Send<TModel>(MailMessage<TModel> mailMessage) where TModel : IMailModel
    {
        var html = await TemplateRenderer.GetTemplateString(mailMessage.Model, mailMessage.Template.TemplatePath);

        var email = new MimeMessage
        {
            Sender = SecureMailboxAddress.Parse(_mailSetting.FromMail),
            Subject = mailMessage.Subject,
            Body = new BodyBuilder { HtmlBody = html }.ToMessageBody()
        };

        email.Sender.Name = _mailSetting.DisplayName;
        email.From.Add(email.Sender);

        foreach (var toEmail in mailMessage.ToEmails)
        {
            email.To.Add(
                toEmail.Display != null
                ? new MailboxAddress(toEmail.Display, toEmail.Value)
                : MailboxAddress.Parse(toEmail.Value)
            );
        }

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_mailSetting.Host, _mailSetting.Port, SecureSocketOptions.StartTlsWhenAvailable);
        await smtp.AuthenticateAsync(_mailSetting.FromMail, _mailSetting.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
        
    }

}
