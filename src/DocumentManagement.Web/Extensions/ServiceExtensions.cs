using System.Runtime.CompilerServices.Extensions;
using System.Security.Claims;
using DocumentManagement.Core.Options;
using DocumentManagement.Persistence.Extensions;
using Elsa.Server.Authentication.Extensions;
using Storage.Net;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DocumentManagement.Web.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ApplicationService(this IServiceCollection services,
        IConfigurationRoot configuration)
    {
        var dbConnectionString = configuration.GetConnectionString("Sqlite");

        // Razor Pages (for UI).
        services.AddRazorPages();

        services.AddControllers();
        
        // Authentication & Authorization.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        
        services.AddElsaJwtBearerAuthentication(options =>
        {
            options.ValidateIssuer = true;
            options.ValidateAudience = true;
            options.ValidateIssuerSigningKey = true;
            options.ValidIssuer = configuration["Jwt:Issuer"];
            options.IssuerSigningKey = configuration["Jwt:SecretKey"];
            options.Audience = configuration["Jwt:Audience"];
        });

        // Add a custom policy.
        services
            .AddAuthorization(auth => auth
                .AddPolicy("IsAdmin", policy => policy
                    .RequireClaim("is-admin", "true")));

        services.Configure<JwtOptions>(options => configuration.GetSection("Jwt").Bind(options));

        // Hangfire (for background tasks).
        services
            .AddHangfire(config => config
                // Use same SQLite database as Elsa for storing jobs. 
                .UseSQLiteStorage(dbConnectionString)
                .UseSimpleAssemblyNameTypeSerializer()

                // Elsa uses NodaTime primitives, so Hangfire needs to be able to serialize them.
                .UseRecommendedSerializerSettings(settings => settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)))
            .AddHangfireServer((sp, options) =>
            {
                // Bind settings from configuration.
                configuration.GetSection("Hangfire").Bind(options);

                // Configure queues for Elsa workflow dispatchers.
                options.ConfigureForElsaDispatchers(sp);
            });

        // Elsa (workflows engine).
        services.AddWorkflowServices(dbContext =>
            dbContext.UseSqlite(dbConnectionString));

        // Configure SMTP.
        services.Configure<SmtpOptions>(options =>
            configuration.GetSection("Elsa:Smtp").Bind(options));

        // Configure HTTP activities.
        services.Configure<HttpActivityOptions>(options =>
            configuration.GetSection("Elsa:Server").Bind(options));

        // Elsa API (to allow Elsa Dashboard to connect for checking workflow instances).
        services.AddElsaApiEndpoints();
        services.AddElsaUserEndpoints();
        services.AddTenantAccessorFromHeader("Content-Disposition");
        services.AddElsaSwagger();
        
        // Domain services.
        AddDomainServices(services);
        
        // Persistence.
        AddPersistenceServices(services, dbConnectionString);
        
        // Allow arbitrary client browser apps to access the API for demo purposes only.
        // In a production environment, make sure to allow only origins you trust.
        services.AddCors(cors => 
            cors.AddDefaultPolicy(policy =>
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .WithExposedHeaders("Content-Disposition")));

        return services;
    }
    private static void AddDomainServices(IServiceCollection services)
    {
        services.AddDomainServices();
    
        // Configure Storage for DocumentStorage.
        services.Configure<DocumentStorageOptions>(options => 
            options.BlobStorageFactory = () => 
                StorageFactory.Blobs.DirectoryFiles(
                    Path.Combine(Environment.CurrentDirectory, "App_Data/Uploads")));
    }
    private static void AddPersistenceServices(IServiceCollection services, string dbConnectionString)
    {
        services.AddDomainPersistence(dbConnectionString);
    }
}