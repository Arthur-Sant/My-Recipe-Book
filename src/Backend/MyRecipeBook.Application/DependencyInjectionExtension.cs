using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Application.Services.AutoMapper;
using MyRecipeBook.Application.UseCases.Dashboard;
using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Application.UseCases.Login.External;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Application.UseCases.Recipe.Image;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Application.UseCases.User.Delete.Delete;
using MyRecipeBook.Application.UseCases.User.Delete.Request;
using MyRecipeBook.Application.UseCases.User.Profile;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Application.UseCases.User.Update;
using Sqids;

namespace MyRecipeBook.Application;
public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddIdEncoder(services, configuration);
        AddAutoMapper(services);
        AddUseCases(services);
    }

    private static void AddAutoMapper(IServiceCollection services)
    {
        services.AddScoped(options => new MapperConfiguration(mapperOption =>
            {
                var sqids = options.GetService<SqidsEncoder<long>>()!;

                mapperOption.AddProfile(new AutoMapping(sqids));
            }).CreateMapper()
        );
    }

    private static void AddIdEncoder(IServiceCollection services, IConfiguration configuration)
    {
        var sqids = new SqidsEncoder<long>(new()
        {
            MinLength = 5,
            Alphabet = configuration.GetValue<string>("Settings:IdCryptographyAlphabet")!
        });

        services.AddSingleton(sqids);
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddScoped<IDoLoginUseCase, DoLoginUsecase>();
        services.AddScoped<IExternalLoginUseCase, ExternalLoginUseCase>();

        services.AddScoped<IRegisterUseCase, RegisterUserUseCase>();
        services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
        services.AddScoped<IUpdateUserUsecase, UpdateUserUsecase>();
        services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
        services.AddScoped<IRequestDeleteUserUseCase, RequestDeleteUserUseCase>();
        services.AddScoped<IDeleteUserAccountUseCase, DeleteUserAccountUseCase>();

        services.AddScoped<IRegisterRecipeUseCase, RegisterRecipeUseCase>();
        services.AddScoped<IFilterRecipeUseCase, FilterRecipeUseCase>();
        services.AddScoped<IGetRecipeByIdUseCase, GetRecipeByIdUseCase>();
        services.AddScoped<IDeleteRecipeUseCase, DeleteRecipeUseCase>();
        services.AddScoped<IUpdateRecipeUseCase, UpdateRecipeUseCase>();
        services.AddScoped<IGenerateRecipeUseCase, GenerateRecipeUseCase>();
        services.AddScoped<IAddUpdateImageCoverUseCase, AddUpdateImageCoverUseCase>();

        services.AddScoped<IGetDashboardUseCase, GetDashboardUseCase>();
    }
}
 