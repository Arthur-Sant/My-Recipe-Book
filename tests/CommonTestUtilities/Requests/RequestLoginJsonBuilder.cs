using Bogus;
using MyRecipeBook.Communication.Requests.Login;

namespace CommonTestUtilities.Requests;
public static class RequestLoginJsonBuilder
{
    public static RequestLoginJson Build(int passwordLenght = 10)
    {
        return new Faker<RequestLoginJson>()
            .RuleFor(user => user.Email, (f) => f.Internet.Email())
            .RuleFor(user => user.Password, (f) => f.Internet.Password(passwordLenght));
    }
}
