namespace MyRecipeBook.Domain.Services.Mail.Schema;

public class MailSetting
{
    public required string FromMail { get; set; }
    public required string DisplayName { get; set; }
    public required string Password { get; set; }
    public required string Host { get; set; }
    public required int Port { get; set; }
}
