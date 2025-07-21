using Moq;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommonTestUtilities.Repositories;

public class RecipeDeleteOnlyRepositoryBuilder
{
    public static IRecipeDeleteOnlyRepository Build() => new Mock<IRecipeDeleteOnlyRepository>().Object;
}
