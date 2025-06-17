using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.User.Register;
public interface IRegisterUseCase
{
    public Task<ResponseRegisterUserJson> Execute(RequestRegisterUserJson body);
}
