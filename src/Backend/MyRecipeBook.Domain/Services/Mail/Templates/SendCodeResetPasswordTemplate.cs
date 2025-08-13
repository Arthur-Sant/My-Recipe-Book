using MyRecipeBook.Domain.Services.Mail.Models;

namespace MyRecipeBook.Domain.Services.Mail.Templates;

public class SendCodeResetPasswordTemplate : IMailTemplate<SendCodeResetPasswordModel>
{
    public string TemplatePath => "SendCodeResetPassword.html";
}