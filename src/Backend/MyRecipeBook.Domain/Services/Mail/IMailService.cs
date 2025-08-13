using MyRecipeBook.Domain.Services.Mail.Models;
using MyRecipeBook.Domain.Services.Mail.Schema;

namespace MyRecipeBook.Domain.Services.Mail;

public interface IMailService
{
    public Task Send<TModel>(MailMessage<TModel> mailMessage) where TModel : IMailModel;
}
