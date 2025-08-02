﻿using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Domain.ValueObjects;

namespace MyRecipeBook.Infrastructure.Services.Storage;

public class StorageService(BlobServiceClient _blobServiceClient) : IStorageService
{
    public Task Delete(User user, string fileName)
    {
        throw new NotImplementedException();
    }

    public Task DeleteContainer(Guid userIdentifier)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetFileUrl(User user, string fileName)
    {
        var containerName = user.UserIdentifier.ToString();

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        var exist = await containerClient.ExistsAsync();
        if(exist.Value.IsFalse())
        {
            return string.Empty;
        }

        var blobClient = containerClient.GetBlobClient(fileName);

        exist = await blobClient.ExistsAsync();
        if(exist.Value)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(MyRecipeBookRuleConstants.MAXIMUM_IMAGE_URL_LIFETIMR_IN_MINUTES)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        return string.Empty;

    }

    public async Task Upload(User user, Stream file, string fileName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());
        await container.CreateIfNotExistsAsync();

        var blobClient = container.GetBlobClient(fileName);

        await blobClient.UploadAsync(file, overwrite: true);
    }
}
