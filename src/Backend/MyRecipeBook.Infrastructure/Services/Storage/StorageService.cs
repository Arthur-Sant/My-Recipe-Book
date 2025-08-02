using Azure.Storage.Blobs;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Infrastructure.Services.Storage;

public class StorageService(BlobServiceClient _blobServiceClient) : IStorageService
{
    public async Task Delete(User user, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());

        var exist = await containerClient.ExistsAsync();
        if(exist.Value)
        {
            await containerClient.DeleteBlobIfExistsAsync(fileName);
        }
    }

    public Task DeleteContainer(Guid userIdentifier)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetFileUrl(User user, string fileName)
    {
        throw new NotImplementedException();
    }

    public async Task Upload(User user, Stream file, string fileName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());
        await container.CreateIfNotExistsAsync();

        var blobClient = container.GetBlobClient(fileName);

        await blobClient.UploadAsync(file, overwrite: true);
    }
}
