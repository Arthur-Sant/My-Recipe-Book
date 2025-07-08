using MyRecipeBook.Application.Services.Cryptography;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin;
public class DoLoginUsecase : IDoLoginUseCase
{
    private readonly IUserReadOnlyRepository _repository;
    private readonly PasswordEncripter _passwordEncripter;

    public DoLoginUsecase(IUserReadOnlyRepository repository, PasswordEncripter passwordEncripter) 
    { 
        _repository = repository;
        _passwordEncripter = passwordEncripter;
    } 

    public async Task<ResponseRegisterUserJson> Execute(RequestLoginJson body)
    {
        var encriptedPassword = _passwordEncripter.Encrypt(body.Password);

        var user = await _repository.GetByEmailAndPassword(body.Email, encriptedPassword) ?? throw new InvalidLoginException();

        return new ResponseRegisterUserJson { Name = user.Name };
    }
}
