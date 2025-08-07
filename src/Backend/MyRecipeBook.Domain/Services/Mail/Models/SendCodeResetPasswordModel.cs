namespace MyRecipeBook.Domain.Services.Mail.Models;

public class SendCodeResetPasswordModel : IMailModel
{
    public required string UserName { get; set; }
    public required string Code { get; set; } 
}
