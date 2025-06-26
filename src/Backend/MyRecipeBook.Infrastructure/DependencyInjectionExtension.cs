using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Infrastructure.DataAccess;
using MyRecipeBook.Infrastructure.DataAccess.Repositories;
using MyRecipeBook.Infrastructure.Extensions;
using System.Reflection;

namespace MyRecipeBook.Infrastructure;
public static class DependencyInjectionExtension
{

    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddRepositories(services);

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
        services.AddScoped<IUserWriteOnlyRepository , UserRepository>();
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
}
