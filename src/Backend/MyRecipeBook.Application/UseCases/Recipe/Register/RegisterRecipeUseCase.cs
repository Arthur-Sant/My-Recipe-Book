using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Register;

public class RegisterRecipeUseCase(
        IRecipeWriteOnlyRepository _repository,
    IMapper _mapper,
    IUnityOfWork _unitOfWork,
    ILoggedUser _loggedUser,
    IStorageService _storageService
    ) : IRegisterRecipeUseCase
{
    public async Task<ResponseRegiteredRecipeJson> Execute(RequestRegisterRecipeFormData body)
    {
        Validate(body);

        var loggedUser = await _loggedUser.User();

        var recipe = _mapper.Map<Domain.Entities.Recipe>(body);
        recipe.UserId = loggedUser.Id;

        var instructions = body.Instructions.OrderBy(i => i.Step).ToList();
        for(var index = 0; index < instructions.Count; index++)
            instructions[index].Step = index + 1;

        recipe.Instructions = _mapper.Map<IList<Domain.Entities.Instruction>>(instructions);

        if(body.Image is not null)
        {

            var fileStream = body.Image.OpenReadStream();

            var (isValidImage, extension) = fileStream.ValidateAndGetImageExtension();

            if(isValidImage.IsFalse())
            {
                throw new ErrorOnValidationException([ResourceMessagesException.ONLY_IMAGES_ACCEPTED]);
            }

            recipe.ImageIdentifier = $"{Guid.NewGuid()}{extension}";

            await _storageService.Upload(loggedUser, fileStream, recipe.ImageIdentifier);
        }

        await _repository.Add(recipe);

        await _unitOfWork.Commit();

        return _mapper.Map<ResponseRegiteredRecipeJson>(recipe);
    }

    private static void Validate(RequestRecipeJson body)
    {
        var result = new RecipeValidator().Validate(body);

        if(result.IsValid.IsFalse())
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).Distinct().ToList());
    }
}
