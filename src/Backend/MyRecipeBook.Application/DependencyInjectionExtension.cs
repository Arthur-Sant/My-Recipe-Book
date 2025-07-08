using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Application.Services.AutoMapper;
using MyRecipeBook.Application.Services.Cryptography;
using MyRecipeBook.Application.UseCases.User.Register;
using Microsoft.Extensions.Configuration;
using MyRecipeBook.Application.UseCases.Login.DoLogin;

namespace MyRecipeBook.Application;
public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddPasswordEncrypter(services, configuration);
        AddAutoMapper(services);
        AddUseCases(services);
    }

    private static void AddAutoMapper(IServiceCollection services)
    {
        services.AddScoped(options => new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapping());
            }).CreateMapper()
        );
    }
    private static void AddUseCases(IServiceCollection services)
    {
        services.AddScoped<IRegisterUseCase, RegisterUserUseCase>();
        services.AddScoped<IDoLoginUseCase, DoLoginUsecase>();
    }

    private static void AddPasswordEncrypter(IServiceCollection services, IConfiguration configuration)
    {
        var additionalKey = configuration.GetValue<string>("Settings:Password:AdditionalKey");

        services.AddScoped(options => new PasswordEncripter(additionalKey!));
    }
}
 