using DocumentManagement.Workflows.Handlers;
using DocumentManagement.Workflows.Scripting.JavaScript;

namespace DocumentManagement.Workflows.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowServices(this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddNotificationHandlersFrom<StartDocumentWorkflows>();
        services.AddElsa(configureDb);

        return services;
    }

    private static IServiceCollection AddElsa(this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDb)
    {
        services
            .AddElsa(elsa => elsa

                // Use EF Core's SQLite provider to store workflow instances and bookmarks.
                .UseEntityFrameworkPersistence(configureDb)
                    
                // Ue Console activities for testing & demo purposes.
                .AddConsoleActivities()

                // Use Hangfire to dispatch workflows from.
                .UseHangfireDispatchers()

                // Configure Email activities.
                .AddEmailActivities()

                // Configure HTTP activities.
                .AddHttpActivities()
                .AddActivitiesFrom<GetDocument>()
                .AddJavaScriptActivities()
            );
        
        // Register custom type definition provider for JS intellisense.
        services.AddJavaScriptTypeDefinitionProvider<CustomTypeDefinitionProvider>();

        // Get directory path to current assembly.
        var currentAssemblyPath = Path.GetDirectoryName(typeof(ServiceCollectionExtensions).Assembly.Location);

        // Configure Storage for BlobStorageWorkflowProvider with a directory on disk from where to load workflow definition JSON files from the local "Workflows" folder.
        services.Configure<BlobStorageWorkflowProviderOptions>(options => 
            options.BlobStorageFactory = () => 
                StorageFactory.Blobs.DirectoryFiles(Path.Combine(currentAssemblyPath, "Workflows")));

        return services;
    }
}