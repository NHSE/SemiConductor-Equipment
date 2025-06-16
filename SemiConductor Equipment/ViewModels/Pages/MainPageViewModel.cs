using System;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Messages;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;
using Wpf.Ui;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class MainPageViewModel : ObservableObject
    {
        #region FIELDS
        private readonly IDateTime _iDateTime;
        private readonly DispatcherTimer _timer;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private string? _currenttime;
        [ObservableProperty]
        private string? _secsdatalog;
        [ObservableProperty]
        private string? _loadport1imagepath;
        [ObservableProperty]
        private string? _loadport2imagepath;


        public Chamber1_ViewModel Chamber1 { get; }
        public Chamber2_ViewModel Chamber2 { get; }
        public Chamber3_ViewModel Chamber3 { get; }
        public Chamber4_ViewModel Chamber4 { get; }
        public Chamber5_ViewModel Chamber5 { get; }
        public Chamber6_ViewModel Chamber6 { get; }
        #endregion

        #region CONSTRUCTOR
        public MainPageViewModel(IDateTime iDateTime)
        {
            _iDateTime = iDateTime ?? throw new ArgumentNullException(nameof(iDateTime));

            // 초기값
            this.Currenttime = _iDateTime.GetCurrentTime()?.ToString();

            // 타이머 설정
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => this.Currenttime = _iDateTime.GetCurrentTime()?.ToString();
            _timer.Start();

            Chamber1 = App.Services.GetService<Chamber1_ViewModel>();
            Chamber2 = App.Services.GetService<Chamber2_ViewModel>();
            Chamber3 = App.Services.GetService<Chamber3_ViewModel>();
            Chamber4 = App.Services.GetService<Chamber4_ViewModel>();
            Chamber5 = App.Services.GetService<Chamber5_ViewModel>();
            Chamber6 = App.Services.GetService<Chamber6_ViewModel>();
        }
        #endregion

        #region COMMANDS
        [RelayCommand]
        private void Run()
        {
            Chamber1.PrepareRun(1);
        }
        #endregion

        #region METHODS
        public void AppendLog(string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Secsdatalog += ($"{text}") + "\n";
            });
        }
        #endregion
    }
}
