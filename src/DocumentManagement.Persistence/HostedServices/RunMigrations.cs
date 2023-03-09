using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace DocumentManagement.Persistence.HostedServices;

/// <summary>
/// Executes EF Core migrations.
/// </summary>
public class RunMigrations : IHostedService
{
    private readonly IDbContextFactory<DocumentDbContext> _dbContextFactory;

    public RunMigrations(IDbContextFactory<DocumentDbContext> dbContextFactoryFactory)
    {
        _dbContextFactory = dbContextFactoryFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.Database.MigrateAsync(cancellationToken);
        await dbContext.DisposeAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}