using Bogus;
using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.Storage;

namespace CommonTestUtilities.Storage;

public class StorageServiceBuilder
{
    private readonly Mock<IStorageService> _mock;

    public StorageServiceBuilder() => _mock = new Mock<IStorageService>();

    public StorageServiceBuilder GetFileUrl(User user, string? fileName)
    {
        if(string.IsNullOrWhiteSpace(fileName))
            return this;

        var faker = new Faker();
        var imageUrl = faker.Image.LoremFlickrUrl();

        _mock.Setup(blobStorage => blobStorage.GetFileUrl(user, fileName)).ReturnsAsync(imageUrl);

        return this;
    }

    public StorageServiceBuilder GetFileUrl(User user, IList<Recipe> recipes)
    {
        foreach(var recipe in recipes)
        {
            GetFileUrl(user, recipe.ImageIdentifier);
        }

        return this;
    }

    public IStorageService Build() => _mock.Object;
}
