using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests;
public class RequestChangePasswordJsonBuilder
{
    public static RequestChangePasswordJson Build(int passwordLenght = 10)
    {
        return new Faker<RequestChangePasswordJson>()
            .RuleFor(u => u.Password, f => f.Internet.Password())
            .RuleFor(u => u.NewPassword, f => f.Internet.Password(passwordLenght));
    }
}
