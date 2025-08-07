using MyRecipeBook.Domain.Services.Mail.Models;
using MyRecipeBook.Domain.Services.Mail.Templates;

namespace MyRecipeBook.Domain.Services.Mail.Schema;

public class MailMessage<TModel> where TModel : IMailModel
{
    public required IList<MailAddress> ToEmails { get; set; } = [];
    public string? Subject { get; set; }
    public required MailTemplate<TModel> Template { get; set; } = default!;
    public required TModel Model { get; set; } = default!;
}
