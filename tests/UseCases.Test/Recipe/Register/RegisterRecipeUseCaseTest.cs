using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.Recipe.Register;

public class RegisterRecipeUseCaseTest
{
    [Fact]
    public async Task Success_Without_Image()
    {
        var (user, _) = UserBuilder.Build();

        var body = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(body);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBeNullOrWhiteSpace();
        result.Title.ShouldBe(body.Title);
    }

    [Fact]
    public async Task Error_Title_Empty()
    {
        var (user, _) = UserBuilder.Build();

        var body = RequestRecipeJsonBuilder.Build();
        body.Title = string.Empty;

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => { await useCase.Execute(body); };


        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
            e => e.ErrorMessages.ShouldHaveSingleItem(),
            e => e.ErrorMessages.ShouldContain(m => m.Equals(ResourceMessagesException.RECIPE_TITLE_EMPTY))
            );
    }

    private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var repository = RecipeWriteOnlyRepositoryBuilder.Build();

        return new RegisterRecipeUseCase(
            repository, 
            mapper,
            unitOfWork, 
            loggedUser
            );
    }
}
