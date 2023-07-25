using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reactive.Bindings;
using Reactive.Bindings.Schedulers;
using TextFileVacuumer.Services;
using TextFileVacuumer.ViewModels;
using TextFileVacuumer.Views;

namespace TextFileVacuumer;

public partial class App : Application
{
    internal static new App Current => (App)Application.Current;

    private static IHostBuilder CreateHostBuilder(string[] args) => Host
        .CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(static (context, configBuilder) =>
        {
            //configBuilder.AddCommandLine(args);
            configBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);      // ローカルでは同じPATHでした。 System.IO.Directory.GetCurrentDirectory()
            //configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        })
        //.UseSerilog(static (hostingContext, services, loggerConfiguration) =>
        //{
        //    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);   // "appsettings.json" からログ設定を読み取ります
        //})
        .ConfigureServices(static (context, services) =>
        {
            services
                .AddHostedService<ApplicationHostService>()
                //.Configure<CommandLineArgs>(context.Configuration)
                //.Configure<AppSettings>(context.Configuration.GetSection(nameof(AppSettings)))

                //.AddSingleton(DialogCoordinator.Instance)
                //.AddSingleton<ISettingsManager, SettingsManager>()
                //.AddSingleton<IAppThemeManager, AppThemeManager>()
                //.AddScoped<ISharedFileFactory, SharedFileFactory>()

                // Publishers
                //.AddSingleton<LocalTortoiseCommandPublisher>()
                //.AddSingleton<RemoteShellCommandPublisher>()

                // Listeners
                //.AddSingleton<DialogMessageListener>()
                //.AddSingleton<JumpListListener>()
                //.AddSingleton<CommandListener>()
                //.AddSingleton<CompositeMessageListener>()

                // Models
                //.AddSingleton<ModelMain>()

                // Windows
                .AddSingleton<MainWindow>()
                .AddSingleton<MainWindowViewModel>()

                // Controls
                //.AddTransient<RemoteDirectoryView>()
                //.AddTransient<RemoteDirectoryViewModel>()
                //.AddTransient<SvnFilesView>()
                //.AddTransient<SvnFilesViewModel>()
                //.AddTransient<FooterView>()
                //.AddTransient<FooterViewModel>()
                //.AddTransient<AuthenticationView>()
                //.AddTransient<AuthenticationViewModel>()
                //.AddTransient<ExternalAppPathView>()
                //.AddTransient<ExternalAppPathViewModel>()
                ;
        });

    private IHost _host = default!;
    private ILogger<App> _logger = default!;
    //private CompositeMessageListener _compositeMessageListener = default!;

    /// <summary>
    /// Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    internal T GetService<T>() where T : class => (_host.Services.GetService(typeof(T)) as T) ?? throw new NullReferenceException(typeof(T).ToString());

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    protected override async void OnStartup(StartupEventArgs e)
    {
        _host = CreateHostBuilder(e.Args).Build();
        _logger = GetService<ILogger<App>>();
        _logger.LogTrace("App.OnStartup() : start");

        await _host.StartAsync();

        ReactivePropertyScheduler.SetDefault(new ReactivePropertyWpfScheduler(Dispatcher));
        //_compositeMessageListener = GetService<CompositeMessageListener>();

        base.OnStartup(e);
        _logger.LogTrace("App.OnStartup() : end");
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    protected override async void OnExit(ExitEventArgs e)
    {
        _logger.LogTrace("App.OnExit()");

        await _host.StopAsync();
        _host.Dispose();
        // Host の Dispose 以降は ILogger が使用できなくなります

        base.OnExit(e);
    }

    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        string type = e.Exception.GetType().ToString();
        string message = e.Exception.Message;
        _logger.LogTrace("App.DispatcherUnhandledException() : Type={Type}, Message={Message}", type, message);

        MessageBox.Show($$"""
            Caught an unhandled exception.
            {{type}} : {{message}}
            """,
            "Exception occurred",
            MessageBoxButton.OK, MessageBoxImage.Error);

        e.Handled = true;
    }
}
