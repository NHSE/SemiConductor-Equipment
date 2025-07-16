using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;
using SemiConductor_Equipment.Views.Windows;
using SemiConductor_Equipment.ViewModels.Windows;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.Views.Pages;
using SemiConductor_Equipment.ViewModels.Pages;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Views.Menus;
using SemiConductor_Equipment.ViewModels.Menus;
using SemiConductor_Equipment.Helpers;
using System.Configuration;
using SemiConductor_Equipment.ViewModels.MessageBox;
using SemiConductor_Equipment.Views.MessageBox;

namespace SemiConductor_Equipment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory)); })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<ApplicationHostService>();

                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<MainPage>();
                services.AddSingleton<MainPageViewModel>();

                services.AddSingleton<LogPage>();
                services.AddSingleton<LogPageViewModel>();
                services.AddSingleton<ILogManager>(provider =>
                                    new LogService(@"C:\Logs"));

                services.AddSingleton<IpSettingMenu>();
                services.AddSingleton<IpSettingViewModel>();
                services.AddSingleton<IConfigManager>(provider => 
                                    new IPSettingService(@"C:\Configs"));
                
                services.AddSingleton<EquipMenu>();
                services.AddSingleton<EquipMenusViewModel>();
                services.AddSingleton<IEquipmentConfigManager>(provider => 
                                    new EquipmentSettingService(@"C:\Configs"));

                services.AddSingleton<IBufferManager, BufferService>();
                services.AddTransient<Buffer_ViewModel>();
                services.AddSingleton<Buffer1_Page>();
                services.AddSingleton<Buffer2_Page>();
                services.AddSingleton<Buffer3_Page>();
                services.AddSingleton<Buffer4_Page>();

                services.AddSingleton<IChamberManager, ChamberService>();
                services.AddSingleton<Chamber1_ViewModel>();
                services.AddSingleton<Chamber2_ViewModel>();
                services.AddSingleton<Chamber3_ViewModel>();
                services.AddSingleton<Chamber4_ViewModel>();
                services.AddSingleton<Chamber5_ViewModel>();
                services.AddSingleton<Chamber6_ViewModel>();

                services.AddSingleton<Chamber1_Page>();
                services.AddSingleton<Chamber2_Page>();
                services.AddSingleton<Chamber3_Page>();
                services.AddSingleton<Chamber4_Page>();
                services.AddSingleton<Chamber5_Page>();
                services.AddSingleton<Chamber6_Page>();
                services.AddSingleton<ChamberStatus>();

                services.AddSingleton<IMessageBox, MessageBoxService>();
                services.AddSingleton<MessageBox_ViewModel>();
                services.AddTransient<MessageBoxWindow>();

                services.AddSingleton<IDatabase<Chamberlogtable>, LogtableService>();
                services.AddSingleton<IDateTime, DateTimeService>();
                services.AddSingleton<IThemeService, ThemeService>();
                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                services.AddSingleton<LoadPort1_ViewModel>();
                services.AddSingleton<LoadPort2_ViewModel>();

                services.AddSingleton<Func<byte, ILoadPortViewModel>>(provider => key =>
                {
                    return key switch
                    {
                        1 => provider.GetRequiredService<LoadPort1_ViewModel>(),
                        2 => provider.GetRequiredService<LoadPort2_ViewModel>(),
                        _ => throw new ArgumentException("Invalid LoadPortId", nameof(key)),
                    };
                });

                services.AddSingleton<LoadPort1_Page>();
                services.AddSingleton<LoadPort2_Page>();
                services.AddSingleton<CarrierSetupWindow>();

                services.AddDbContext<LogDatabaseContext>();

                services.AddSingleton<Action<string>>(provider => AppendLog);
                services.AddSingleton<MessageHandlerService>();
                services.AddSingleton<ISecsGemServer, SecsGemServer>();
                services.AddSingleton<WaferService>();
                services.AddSingleton<WaferProcessCoordinatorService>();
                services.AddSingleton<LoadPortService>();
                services.AddSingleton<IRobotArmManager, RobotArmService>();
                services.AddSingleton<RunningStateService>();
                services.AddSingleton<DbLogHelper>();
            }).Build();

        public static Action<string> AppendLog = msg => Console.WriteLine(msg);

        /// <summary>
        /// Gets services.
        /// </summary>
        public static IServiceProvider Services
        {
            get { return _host.Services; }
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            // UI 스레드 예외 처리
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;

            // 백그라운드 스레드 예외 처리
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Task 예외 처리
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"[UI 예외]\n{e.Exception.Message}", "예외 발생");
            e.Handled = true;  // 앱 종료 방지
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show($"[도메인 예외]\n{ex?.Message}", "예외 발생");
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show($"[Task 예외]\n{e.Exception.Message}", "예외 발생");
            e.SetObserved();
        }
    }
}
