using AutoMapper;
using MyRecipeBook.Application.Services.Cryptography;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserUseCase : IRegisterUseCase
{
    private readonly IUserWriteOnlyRepository _writeonlyRepository;
    private readonly IUserReadOnlyRepository _readonlyRepository;
    private readonly IUnityOfWork _unityOfWork;
    private readonly IMapper _mapper;
    private readonly PasswordEncripter _passwordEncripter;

    public RegisterUserUseCase(
        IUserWriteOnlyRepository writeonlyRepository, 
        IUserReadOnlyRepository readonlyRepository,
        IMapper mapper,
        PasswordEncripter passwordEncripter,
        IUnityOfWork unityOfWork
        )
    {
        _writeonlyRepository = writeonlyRepository;
        _readonlyRepository = readonlyRepository;
        _mapper = mapper;
        _passwordEncripter = passwordEncripter;
        _unityOfWork = unityOfWork;
    }

    public async Task<ResponseRegisterUserJson> Execute(RequestRegisterUserJson body)
    {
        await Validate(body);

        var user = _mapper.Map<Domain.Entities.User>(body);
        user.Password = _passwordEncripter.Encrypt(body.Password);

        await _writeonlyRepository.Add(user);

        await _unityOfWork.Commit();

        return new ResponseRegisterUserJson{
            Name = user.Name,
        };
    }

    private async Task Validate(RequestRegisterUserJson body)
    {
        var result = new RegisterUserValidator().Validate(body);

        var emailExist = await _readonlyRepository.ExistActiveUserWithEmail(body.Email);

        if(emailExist)
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessagesException.EMAIL_ALREADY_REGISTERED));
        }

        if(result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
