using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Services
{
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public ApplicationHostService(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ApplicationHostService: StartAsync 진입");
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                System.Diagnostics.Debug.WriteLine("ApplicationHostService: MainWindow 인스턴스 생성 성공");
                Application.Current.MainWindow = mainWindow; // WPF의 MainWindow 속성도 설정
                mainWindow.ShowWindow();
                System.Diagnostics.Debug.WriteLine("ApplicationHostService: MainWindow.Show() 호출 완료");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ApplicationHostService: 예외 발생: " + ex.ToString());
                System.Windows.MessageBox.Show(ex.ToString(), "예외 발생");
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
    }
}
