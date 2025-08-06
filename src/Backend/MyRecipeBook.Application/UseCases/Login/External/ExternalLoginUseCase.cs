using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Application.UseCases.Login.External;

public class ExternalLoginUseCase(
    IUserReadOnlyRepository _repository,
    IUserWriteOnlyRepository _repositoryWrite,
    IUnityOfWork _unitOfWork,
    IAccessTokenGenerator _accessTokenGenerator
    ) : IExternalLoginUseCase
{
    public async Task<string> Execute(string name, string email)
    {
        var user = await _repository.GetByEmail(email);

        if(user is null)
        {
            user = new Domain.Entities.User
            {
                Name = name,
                Email = email,
                Password = "-"
            };

            await _repositoryWrite.Add(user);
            await _unitOfWork.Commit();
        }

        return _accessTokenGenerator.Generate(user.UserIdentifier);
    }
}
