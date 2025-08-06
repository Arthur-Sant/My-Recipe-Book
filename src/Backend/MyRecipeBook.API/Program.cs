using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyRecipeBook.API.BackgroundServices;
using MyRecipeBook.API.Convertes;
using MyRecipeBook.API.Filters;
using MyRecipeBook.API.Middleware;
using MyRecipeBook.API.Token;
using MyRecipeBook.Application;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure;
using MyRecipeBook.Infrastructure.Extensions;
using MyRecipeBook.Infrastructure.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new StringConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Services.AddSwaggerGen(options =>
{
    var tokenType = "Bearer";

    options.OperationFilter<IdsFilter>();

    options.AddSecurityDefinition(tokenType, new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token is the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type =SecuritySchemeType.ApiKey,
        Scheme = tokenType
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = tokenType
                },
                Scheme = "oauth2",
                Name = tokenType,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Configuration.GetConnectionString("DataBaseType");

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ITokenProvider, HttpContextTokenValue>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

if(builder.Configuration.IsUnitTestEnviroment().IsFalse())
{
    builder.Services.AddHostedService<DeleteUserService>();

    AddGoogleAuthentication();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

MigrateDatabase();

await app.RunAsync();

void MigrateDatabase()
{
    if(builder.Configuration.IsUnitTestEnviroment())
        return;

    var databaseType = builder.Configuration.DatabaseType();
    var connectionString = builder.Configuration.ConnectionString();

    var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

    DatabaseMigrations.Migrate(databaseType, connectionString, serviceScope.ServiceProvider);
}

void AddGoogleAuthentication()
{
    var clientId = builder.Configuration.GetValue<string>("Settings:Google:ClientId")!;
    var clientSecret = builder.Configuration.GetValue<string>("Settings:Google:ClientSecret")!;

    builder.Services.AddAuthentication(config =>
    {
        config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddCookie()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = clientId;
        googleOptions.ClientSecret = clientSecret;
    });
}

public partial class Program {
    protected Program() { }
}
