using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.CodeToPerformAction;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.Mail;
using MyRecipeBook.Domain.Services.Mail.Models;
using MyRecipeBook.Domain.Services.Mail.Schema;
using MyRecipeBook.Domain.Services.Mail.Templates;
using System.Security.Cryptography;
using System.Text;

namespace MyRecipeBook.Application.UseCases.Login.RequestCode;

public class RequestCodeResetPasswordUseCase(
    IUserReadOnlyRepository _userReadRepository,
    ICodeToPerformActionWriteOnlyRepository _repositoryWrite,
    ICodeToPerformActionReadOnlyRepository _repositoryRead,
    IMailService _mailService,
    IUnityOfWork _unityOfWork
    ) : IRequestCodeResetPasswordUseCase
{
    public async Task Execute(string email)
    {
        var user = await _userReadRepository.GetByEmail(email);
        
        if(user is not null)
        {
            var code = await GenerateCode();

            var codeToPerformáction = new CodeToPerformAction
            {
                Value = code,
                UserId = user.Id
            };
       
            await _repositoryWrite.Add(codeToPerformáction);
       
            await _unityOfWork.Commit();

            var mailMessage = new MailMessage<SendCodeResetPasswordModel>
            {
                ToEmails = [new MailAddress { Value = user.Email }],
                Template = new SendCodeResetPasswordTemplate(),
                Subject = "RESET PASSWORD",
                Model = new SendCodeResetPasswordModel
                {
                    Code = code,
                    UserName = user.Name
                }
            };

            await _mailService.Send(mailMessage);
        }
    }

    private async Task<string> GenerateCode()
    {
        string code;
        bool existCode;

        do
        {
            code = GenerateSecureCode(6);
            existCode = await _repositoryRead.ExistCode(code);
        }
        while(existCode);

        return code;
    }

    private static string GenerateSecureCode(int length)
    {
        using var rng = RandomNumberGenerator.Create();

        var codeBuilder = new StringBuilder(length);

        byte[] buffer = new byte[1];

        while(codeBuilder.Length < length)
        {
            rng.GetBytes(buffer);
            int digit = buffer[0] % 10;
            codeBuilder.Append(digit);
        }

        return codeBuilder.ToString();
    }
}
