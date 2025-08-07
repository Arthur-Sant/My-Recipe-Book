using MyRecipeBook.Domain.Services.Mail.Models;

namespace MyRecipeBook.Domain.Services.Mail.Templates;

public class SendCodeResetPasswordTemplate : MailTemplate<SendCodeResetPasswordModel>
{
    public override string TemplatePath => "SendCodeResetPassword.html";
}