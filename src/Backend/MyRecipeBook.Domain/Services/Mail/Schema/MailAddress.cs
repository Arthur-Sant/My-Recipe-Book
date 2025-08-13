namespace MyRecipeBook.Domain.Services.Mail.Schema;

public class MailAddress
{
    public required string Value { get; set; }
    public string? Display {  get; set; }
}
