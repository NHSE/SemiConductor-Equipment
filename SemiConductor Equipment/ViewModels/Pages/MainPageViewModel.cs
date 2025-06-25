using System;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Messages;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.Views.Menus;
using SemiConductor_Equipment.Views.Pages;
using SemiConductor_Equipment.Views.Windows;
using Wpf.Ui;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class MainPageViewModel : ObservableObject
    {
        #region FIELDS
        private readonly IDateTime _iDateTime;
        private readonly ILogManager _logmanager;
        private readonly IConfigManager _configManager;
        private readonly DispatcherTimer _timer;
        private readonly MessageHandlerService _messageHandler;
        private readonly RunningStateService _runningStateService;
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
        [ObservableProperty]
        private Brush? _equipment_color;
        [ObservableProperty]
        private string? _equipment_state;
        [ObservableProperty]
        private bool? _isConnected;

        #endregion

        #region CONSTRUCTOR
        public MainPageViewModel(IDateTime iDateTime, ILogManager logmanager, IConfigManager configManager, MessageHandlerService messageHandler, RunningStateService runningStateService)
        {
            _iDateTime = iDateTime ?? throw new ArgumentNullException(nameof(iDateTime));

            // 초기값
            this.Currenttime = _iDateTime.GetCurrentTime()?.ToString();

            // 타이머 설정
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => this.Currenttime = _iDateTime.GetCurrentTime()?.ToString();
            _timer.Start();

            this._runningStateService = runningStateService;
            this._runningStateService.DataChange += OnEquipment_State_Change;

            this._logmanager = logmanager;
            this._messageHandler = messageHandler;
            this._configManager = configManager;

            this.Equipment_color = Brushes.LightBlue;
            this.Equipment_state = "Ready";

            SecsGemServer.Initialize(AppendLog, messageHandler, _configManager.IP, _configManager.Port, _configManager.DeviceID);

            WeakReferenceMessenger.Default.Register<ViewModelMessages>(this, (r, m) =>
            {
                switch (m.Content)
                {
                    case "LoadPort1_in_wafer":
                        this.Loadport1imagepath = "/Resources/Carrier_in_wafer.png";
                        break;
                    case "LoadPort1":
                        this.Loadport1imagepath = "/Resources/Carrier_nothing.png";
                        break;
                    case "LoadPort2_in_wafer":
                        this.Loadport2imagepath = "/Resources/Carrier_in_wafer.png";
                        break;
                    case "LoadPort2":
                        this.Loadport2imagepath = "/Resources/Carrier_nothing.png";
                        break;
                        // 추가 분기 가능
                }
            });
        }
        #endregion

        #region COMMANDS
        [RelayCommand]
        private void LoadPort1() => NavigateToPage<LoadPort1_Page>();

        [RelayCommand]
        private void LoadPort2() => NavigateToPage<LoadPort2_Page>();

        [RelayCommand]
        private void Buffer1() => NavigateToPage<Buffer1_Page>();
        [RelayCommand]
        private void Buffer2() => NavigateToPage<Buffer2_Page>();
        [RelayCommand]
        private void Buffer3() => NavigateToPage<Buffer3_Page>();
        [RelayCommand]
        private void Buffer4() => NavigateToPage<Buffer4_Page>();

        [RelayCommand]
        private void Chamber1() => NavigateToPage<Chamber1_Page>();
        [RelayCommand]
        private void Chamber2() => NavigateToPage<Chamber2_Page>();
        [RelayCommand]
        private void Chamber3() => NavigateToPage<Chamber3_Page>();
        [RelayCommand]
        private void Chamber4() => NavigateToPage<Chamber4_Page>();
        [RelayCommand]
        private void Chamber5() => NavigateToPage<Chamber5_Page>();
        [RelayCommand]
        private void Chamber6() => NavigateToPage<Chamber6_Page>();

        [RelayCommand]
        private void SubMenuLog()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Source = new Uri("../Menus/LogPage.xaml", UriKind.Relative);
            }
        }

        [RelayCommand]
        private void SubMenuIPSetting()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var ipSettingPage = App.Services.GetRequiredService<IpSettingMenu>(); // DI로 생성된 인스턴스 사용
                mainWindow.MainFrame.Navigate(ipSettingPage);
            }
        }
        #endregion

        #region METHODS

        private void OnEquipment_State_Change(object sender, EquipmentStatusEnum state)
        {
            if (state == EquipmentStatusEnum.Running)
            {
                this.Equipment_color = Brushes.Orange;
                this.Equipment_state = "Running";
            }
            else if(state == EquipmentStatusEnum.Completed)
            {
                this.Equipment_color = Brushes.LimeGreen;
                this.Equipment_state = "Completed";
            }
            else if(state == EquipmentStatusEnum.Error)
            {
                this.Equipment_color = Brushes.Red;
                this.Equipment_state = "Error";
            }
            else
            {
                this.Equipment_color = Brushes.LightBlue;
                this.Equipment_state = "Ready";
            }
        }

        private void NavigateToPage<TPage>() where TPage : class
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var page = App.Services.GetRequiredService<TPage>();
                mainWindow.MainFrame.Navigate(page);
            }
        }

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
