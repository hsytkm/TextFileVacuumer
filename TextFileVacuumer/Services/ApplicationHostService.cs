using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TextFileVacuumer.Views;

namespace TextFileVacuumer.Services;

/// <summary>
/// Managed host of the application.
/// </summary>
public sealed class ApplicationHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ApplicationHostService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var mainWindow = _serviceProvider.GetService<MainWindow>()!;
        mainWindow.Show();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
