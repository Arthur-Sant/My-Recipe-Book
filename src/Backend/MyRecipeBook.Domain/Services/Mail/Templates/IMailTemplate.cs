using MyRecipeBook.Domain.Services.Mail.Models;

namespace MyRecipeBook.Domain.Services.Mail.Templates;

public interface IMailTemplate<TModel> where TModel : IMailModel
{
    public string TemplatePath { get; }

    public Type ModelType => typeof(TModel);
}
