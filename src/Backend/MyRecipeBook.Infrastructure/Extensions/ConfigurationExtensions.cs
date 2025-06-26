using Microsoft.Extensions.Configuration;
using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Infrastructure.Extensions;
public static class ConfigurationExtensions
{
    public static bool IsUnitTestEnviroment(this IConfiguration configuration)
    {
        return configuration.GetValue<bool>("InMemoryTest");
    }

    public static string ConnectionString(this IConfiguration configuration)
    {
        var databaseType = configuration.DatabaseType();

        if(databaseType == DataBaseType.MySql)
            return configuration.GetConnectionString("ConnectionMySql")!;
        else
            return configuration.GetConnectionString("ConnectionSqlServer")!;
    }

    public static DataBaseType DatabaseType(this IConfiguration configuration)
    {
        var dataBaseType = configuration.GetConnectionString("DatabaseType");

        return (DataBaseType)Enum.Parse(typeof(DataBaseType), dataBaseType!);

    }
}
