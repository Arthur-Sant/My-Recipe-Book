using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.CodeToPerformAction;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.Mail;
using MyRecipeBook.Domain.Services.Mail.Models;
using MyRecipeBook.Domain.Services.Mail.Schema;
using MyRecipeBook.Domain.Services.Mail.Templates;

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
        var random = new Random();
        string sixDigitCode;
        bool existCode;

        do
        {
            sixDigitCode = random.Next(100000, 999999).ToString();
            existCode = await _repositoryRead.ExistCode(sixDigitCode);
        }
        while(existCode); // repete se já existe

        return sixDigitCode;

    }
}
