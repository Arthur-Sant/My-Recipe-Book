using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.ServiceBus;

namespace MyRecipeBook.Application.UseCases.User.Delete.Request;

public class RequestDeleteUserUseCase(
    IDeleteUserQueue _queue,
    IUserUpdateOnlyRepository _userUpdateRepository,
    ILoggedUser _loggedUser,
    IUnityOfWork _unitOfWork
    ) : IRequestDeleteUserUseCase
{
    public async Task Execute()
    {
        var loggedUser = await _loggedUser.User();

        var user = await _userUpdateRepository.GetById(loggedUser.Id);

        user.Active = false;
        _userUpdateRepository.Update(user);

        await _unitOfWork.Commit();

        await _queue.SendMessage(loggedUser);
    }
}
