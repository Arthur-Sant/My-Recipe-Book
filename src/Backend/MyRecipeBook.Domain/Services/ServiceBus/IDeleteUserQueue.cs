using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Domain.Services.ServiceBus;

public interface IDeleteUserQueue
{
    public Task SendMessage(User user);
}
