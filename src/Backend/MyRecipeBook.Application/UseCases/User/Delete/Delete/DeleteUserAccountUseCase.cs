
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Application.UseCases.User.Delete.Delete;

public class DeleteUserAccountUseCase(
    IUserDeleteOnlyRepository _repository,
    IStorageService _storageService,
    IUnityOfWork _unitOfWork
    ) : IDeleteUserAccountUseCase
{
    public async Task Execute(Guid userIdentifier)
    {
        await _storageService.DeleteContainer(userIdentifier);

        await _repository.DeleteAccount(userIdentifier);

        await _unitOfWork.Commit();
    }
}
