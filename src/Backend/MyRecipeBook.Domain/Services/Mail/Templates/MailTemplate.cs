using MyRecipeBook.Domain.Services.Mail.Models;

namespace MyRecipeBook.Domain.Services.Mail.Templates;

public abstract class MailTemplate<TModel> where TModel : IMailModel
{
    public abstract string TemplatePath { get; }
}
