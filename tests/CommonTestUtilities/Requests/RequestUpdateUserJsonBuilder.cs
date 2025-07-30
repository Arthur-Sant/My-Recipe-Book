using Bogus;
using MyRecipeBook.Communication.Requests.User;

namespace CommonTestUtilities.Requests;
public class RequestUpdateUserJsonBuilder
{
    public static RequestUpdateUserJson Build()
    {
        return new Faker<RequestUpdateUserJson>()
            .RuleFor(user => user.Name, f => f.Person.FirstName)
            .RuleFor(user => user.Email, (f, u) => f.Internet.Email(u.Email));
    }
}
