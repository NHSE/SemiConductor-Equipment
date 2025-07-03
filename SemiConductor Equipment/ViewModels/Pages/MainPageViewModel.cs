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
        private readonly ISecsGemServer _secsGemServer;
        private readonly IChamberManager _chamberManager;
        private readonly IBufferManager _bufferManager;
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
        private bool? _isConnected = false;
        [ObservableProperty]
        private bool? _isDisconnected = false;

        [ObservableProperty]
        private string? _chamber1_state;
        [ObservableProperty]
        private Brush? _chamber1_color;
        [ObservableProperty]
        private string? _chamber2_state;
        [ObservableProperty]
        private Brush? _chamber2_color;
        [ObservableProperty]
        private string? _chamber3_state;
        [ObservableProperty]
        private Brush? _chamber3_color;
        [ObservableProperty]
        private string? _chamber4_state;
        [ObservableProperty]
        private Brush? _chamber4_color;
        [ObservableProperty]
        private string? _chamber5_state;
        [ObservableProperty]
        private Brush? _chamber5_color;
        [ObservableProperty]
        private string? _chamber6_state;
        [ObservableProperty]
        private Brush? _chamber6_color;

        [ObservableProperty]
        private string? _buffer1_state;
        [ObservableProperty]
        private Brush? _buffer1_color;
        [ObservableProperty]
        private string? _buffer2_state;
        [ObservableProperty]
        private Brush? _buffer2_color;
        [ObservableProperty]
        private string? _buffer3_state;
        [ObservableProperty]
        private Brush? _buffer3_color;
        [ObservableProperty]
        private string? _buffer4_state;
        [ObservableProperty]
        private Brush? _buffer4_color;

        #endregion

        #region CONSTRUCTOR
        public MainPageViewModel(IDateTime iDateTime, ILogManager logmanager, IConfigManager configManager,
            ISecsGemServer secsGemServer, MessageHandlerService messageHandler, RunningStateService runningStateService, IChamberManager chamberManager, IBufferManager bufferManager)
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
            this._secsGemServer = secsGemServer;
            this._chamberManager = chamberManager;
            this._bufferManager = bufferManager;

            this.Equipment_color = Brushes.LightBlue;
            this.Equipment_state = "Ready";

            this.Chamber1_state = this._chamberManager.Chamber_State["Chamber1"];
            this.Chamber2_state = this._chamberManager.Chamber_State["Chamber2"];
            this.Chamber3_state = this._chamberManager.Chamber_State["Chamber3"];
            this.Chamber4_state = this._chamberManager.Chamber_State["Chamber4"];
            this.Chamber5_state = this._chamberManager.Chamber_State["Chamber5"];
            this.Chamber6_state = this._chamberManager.Chamber_State["Chamber6"];

            this.Buffer1_state = this._bufferManager.Buffer_State["Buffer1"];
            this.Buffer2_state = this._bufferManager.Buffer_State["Buffer2"];
            this.Buffer3_state = this._bufferManager.Buffer_State["Buffer3"];
            this.Buffer4_state = this._bufferManager.Buffer_State["Buffer4"];

            Draw_Color("ALL");

            this._chamberManager.DataEnqueued += Chamber_DataEnqueued;
            this._bufferManager.DataEnqueued += Buffer_DataEnqueued;

            if (this._secsGemServer.Initialize(AppendLog, messageHandler, _configManager))
            {
                this.IsDisconnected = true;
            }

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
        private void DisConnect()
        {
            var configManager = App.Services.GetRequiredService<IConfigManager>();
            this._secsGemServer.Initialize(AppendLog, this._messageHandler, configManager);
            this.IsConnected = true;
            this.IsDisconnected = false;
        }

        [RelayCommand]
        private void Connect()
        {
            var configManager = App.Services.GetRequiredService<IConfigManager>();
            this._secsGemServer.Initialize(AppendLog, this._messageHandler, configManager);
            this.IsConnected = false;
            this.IsDisconnected = true;
        }

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
        private void SubMenuIPSetting() => NavigateToPage<IpSettingMenu>();
        #endregion

        #region METHODS

        private void OnEquipment_State_Change(object sender, EquipmentStatusEnum state)
        {
            if (state == EquipmentStatusEnum.Running)
            {
                this.Equipment_color = Brushes.Orange;
                this.Equipment_state = "Running";
            }
            else if (state == EquipmentStatusEnum.Completed)
            {
                this.Equipment_color = Brushes.LimeGreen;
                this.Equipment_state = "Completed";
            }
            else if (state == EquipmentStatusEnum.Error)
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

        private void Draw_Color(string Draw_Type)
        {
            if (Draw_Type == "Chamber")
                Draw_Chamber_Color();
            else if (Draw_Type == "Buffer")
                Draw_Buffer_Color();
            else
            {
                Draw_Chamber_Color();
                Draw_Buffer_Color();
            }
        }

        private void Draw_Buffer_Color()
        {
            if (this.Buffer1_state == "UN USE")
                this.Buffer1_color = Brushes.DarkGray;
            else
                this.Buffer1_color = Brushes.DarkOrange;

            if (this.Buffer2_state == "UN USE")
                this.Buffer2_color = Brushes.DarkGray;
            else
                this.Buffer2_color = Brushes.DarkOrange;

            if (this.Buffer3_state == "UN USE")
                this.Buffer3_color = Brushes.DarkGray;
            else
                this.Buffer3_color = Brushes.DarkOrange;

            if (this.Buffer4_state == "UN USE")
                this.Buffer4_color = Brushes.DarkGray;
            else
                this.Buffer4_color = Brushes.DarkOrange;
        }

        private void Draw_Chamber_Color()
        {
            if (this.Chamber1_state == "IDLE")
                this.Chamber1_color = Brushes.DarkGray;
            else
                this.Chamber1_color = Brushes.DarkOrange;

            if (this.Chamber2_state == "IDLE")
                this.Chamber2_color = Brushes.DarkGray;
            else
                this.Chamber2_color = Brushes.DarkOrange;

            if (this.Chamber3_state == "IDLE")
                this.Chamber3_color = Brushes.DarkGray;
            else
                this.Chamber3_color = Brushes.DarkOrange;

            if (this.Chamber4_state == "IDLE")
                this.Chamber4_color = Brushes.DarkGray;
            else
                this.Chamber4_color = Brushes.DarkOrange;

            if (this.Chamber5_state == "IDLE")
                this.Chamber5_color = Brushes.DarkGray;
            else
                this.Chamber5_color = Brushes.DarkOrange;

            if (this.Chamber6_state == "IDLE")
                this.Chamber6_color = Brushes.DarkGray;
            else
                this.Chamber6_color = Brushes.DarkOrange;
        }

        public void AppendLog(string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Secsdatalog += ($"{text}") + "\n";
            });
        }

        private void Chamber_DataEnqueued(object? sender, ChamberStatus e)
        {
            switch (e.ChamberName)
            {
                case "Chamber1":
                    this.Chamber1_state = e.State;
                    break;

                case "Chamber2":
                    this.Chamber2_state = e.State;
                    break;

                case "Chamber3":
                    this.Chamber3_state = e.State;
                    break;

                case "Chamber4":
                    this.Chamber4_state = e.State;
                    break;

                case "Chamber5":
                    this.Chamber5_state = e.State;
                    break;

                case "Chamber6":
                    this.Chamber6_state = e.State;
                    break;
            }

            Draw_Color("Chamber");
        }

        private void Buffer_DataEnqueued(object? sender, BufferStatus e)
        {
            switch (e.BufferName)
            {
                case "Buffer1":
                    this.Buffer1_state = e.State;
                    break;

                case "Buffer2":
                    this.Buffer2_state = e.State;
                    break;

                case "Buffer3":
                    this.Buffer3_state = e.State;
                    break;

                case "Buffer4":
                    this.Buffer4_state = e.State;
                    break;
            }
            Draw_Color("Buffer");
            #endregion
        }
    }
}
