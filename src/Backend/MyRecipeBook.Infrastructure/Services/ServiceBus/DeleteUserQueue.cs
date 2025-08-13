using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.ServiceBus;
using Azure.Messaging.ServiceBus;

namespace MyRecipeBook.Infrastructure.Services.ServiceBus;

public class DeleteUserQueue(ServiceBusSender _serviceBusSender ) : IDeleteUserQueue
{
    public async Task SendMessage(User user)
    {
        await _serviceBusSender.SendMessageAsync(new ServiceBusMessage(user.UserIdentifier.ToString()));
    }
}
