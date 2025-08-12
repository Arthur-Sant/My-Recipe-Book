using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mscc.GenerativeAI;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.CodeToPerformAction;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.Services.AI;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Mail;
using MyRecipeBook.Domain.Services.Mail.Schema;
using MyRecipeBook.Domain.Services.ServiceBus;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Infrastructure.DataAccess;
using MyRecipeBook.Infrastructure.DataAccess.Repositories;
using MyRecipeBook.Infrastructure.Extensions;
using MyRecipeBook.Infrastructure.Security.Cryptography;
using MyRecipeBook.Infrastructure.Security.Tokens.Acess.Generator;
using MyRecipeBook.Infrastructure.Security.Tokens.Acess.Validator;
using MyRecipeBook.Infrastructure.Security.Tokens.Refresh;
using MyRecipeBook.Infrastructure.Services.LoggedUser;
using MyRecipeBook.Infrastructure.Services.Mail;
using MyRecipeBook.Infrastructure.Services.OpenAI;
using MyRecipeBook.Infrastructure.Services.ServiceBus;
using MyRecipeBook.Infrastructure.Services.Storage;
using System.Reflection;

namespace MyRecipeBook.Infrastructure;
public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddRepositories(services);
        AddPasswordEncrypter(services);
        AddLoggedUser(services);
        AddTokens(services, configuration);
        AddAI(services, configuration);
        AddStorage(services, configuration);
        AddServiceBus(services, configuration);
        AddMail(services, configuration);

        if(configuration.IsUnitTestEnviroment())
            return;

        var dataBaseType = configuration.GetConnectionString("DatabaseType");

        var databaseTypeEnum = (DataBaseType)Enum.Parse(typeof(DataBaseType), dataBaseType!);

        if(databaseTypeEnum == DataBaseType.MySql)
        {
            AddDbContext_MySql(services, configuration);
            AddFluentMigrator_MySql(services, configuration);
        }
        else
        {
            AddDbContext_SqlServer(services, configuration);
            AddFluentMigrator_SqlServer(services, configuration);
        }

    }

    private static void AddDbContext_SqlServer(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ConnectionSqlServer");
        services.AddDbContext<MyRecipeBookDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }

    private static void AddDbContext_MySql(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ConnectionMySql");
        var serverVersion = new MySqlServerVersion(new Version(9,0,1));

        services.AddDbContext<MyRecipeBookDbContext>(options =>
        {
            options.UseMySql(connectionString, serverVersion);
        });
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnityOfWork, UnityOfWork>();

        services.AddScoped<IUserReadOnlyRepository, UserRepository>();
        services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
        services.AddScoped<IUserDeleteOnlyRepository, UserRepository>();

        services.AddScoped<IRecipeWriteOnlyRepository, RecipeRepository>();
        services.AddScoped<IRecipeReadOnlyRepository, RecipeRepository>();
        services.AddScoped<IRecipeDeleteOnlyRepository, RecipeRepository>();
        services.AddScoped<IRecipeUpdateOnlyRepository, RecipeRepository>();

        services.AddScoped<ICodeToPerformActionReadOnlyRepository, CodeToPerformActionRepository>();
        services.AddScoped<ICodeToPerformActionWriteOnlyRepository, CodeToPerformActionRepository>();
        services.AddScoped<ICodeToPerformActionDeleteOnlyRepository, CodeToPerformActionRepository>();

        services.AddScoped<ITokenReadOnlyRepository, TokenRepository>();
        services.AddScoped<ITokenWriteOnlyRepository, TokenRepository>();
        services.AddScoped<ITokenDeleteOnlyRepository, TokenRepository>();
    }

    private static void AddFluentMigrator_MySql(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.ConnectionString();

        services.AddFluentMigratorCore().ConfigureRunner(options =>
        {
            options
            .AddMySql5()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(Assembly.Load("MyRecipeBook.Infrastructure")).For.All();
        });
    }

    private static void AddFluentMigrator_SqlServer(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.ConnectionString();

        services.AddFluentMigratorCore().ConfigureRunner(options =>
        {
            options
            .AddSqlServer()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(Assembly.Load("MyRecipeBook.Infrastructure")).For.All();
        });
    }

    private static void AddLoggedUser(IServiceCollection services)
    {
        services.AddScoped<ILoggedUser, LoggedUser>();
    }

    private static void AddTokens(IServiceCollection services, IConfiguration configuration)
    {
        var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpirationTimeMinues");
        var signinKey = configuration.GetValue<string>("Settings:Jwt:SigninKey");

        services.AddScoped<IAccessTokenGenerator>(option => new JwtTokenGenerator(expirationTimeMinutes, signinKey!));    
        services.AddScoped<IAccessTokenValidator>(option => new JwtTokenValidator(signinKey!));

        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
    }

    private static void AddPasswordEncrypter(IServiceCollection services)
    {
        services.AddScoped<IPasswordEncripter, PasswordEncripter>();
    }

    private static void AddAI(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IGenerateRecipeAI, AIService>();

        var key = configuration.GetValue<string>("Settings:Google:AI:ApiKey");

        services.AddScoped<IGenerativeAI>(option => new GoogleAI(apiKey: key));
    }

    private static void AddStorage(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("Settings:Storage:Azure");

        if(connectionString.NotEmpty())
        {
            services.AddScoped<IStorageService>(_ => new StorageService(new BlobServiceClient(connectionString)));
        }
    }

    private static void AddServiceBus(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("Settings:ServiceBus:DeleteUserAccount");

        if(string.IsNullOrWhiteSpace(connectionString))
            return;

        var client = new ServiceBusClient(connectionString, new ServiceBusClientOptions
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        });

        var queueName = "user";

        var deleteQueue = new DeleteUserQueue(client.CreateSender(queueName));

        var deleteUserProcessor = new DeleteUserProcessor(client.CreateProcessor(queueName, new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1
        }));

        services.AddSingleton(deleteUserProcessor);

        services.AddScoped<IDeleteUserQueue>(_ => deleteQueue);
    }

    private static void AddMail(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MailSetting>(configuration.GetSection("Settings:Mail").Bind);
        services.AddScoped<IMailService, MailService>();
    }

}
