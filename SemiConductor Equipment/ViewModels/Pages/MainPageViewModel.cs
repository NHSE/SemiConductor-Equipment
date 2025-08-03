using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Secs4Net;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Messages;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.Views.Menus;
using SemiConductor_Equipment.Views.MessageBox;
using SemiConductor_Equipment.Views.Pages;
using SemiConductor_Equipment.Views.Windows;
using Wpf.Ui;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly IRobotArmManager _robotArmManager;
        private readonly DispatcherTimer _timer;
        private readonly MessageHandlerService _messageHandler;
        private readonly RunningStateService _runningStateService;
        private readonly IVIDManager _vIDManager;
        public Dictionary<string, Point> locationPositions = new();
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

        [ObservableProperty]
        private bool? _setting_menu = true;

        [ObservableProperty]
        private ObservableCollection<Wafer> _animation_wafers = new();
        #endregion

        #region CONSTRUCTOR
        public MainPageViewModel(IDateTime iDateTime, ILogManager logmanager, IConfigManager configManager,
            ISecsGemServer secsGemServer, MessageHandlerService messageHandler, RunningStateService runningStateService, 
            IChamberManager chamberManager, IBufferManager bufferManager, IRobotArmManager robotArmManager, IVIDManager svIDManager)
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
            this._robotArmManager = robotArmManager;
            this._vIDManager = svIDManager;

            this.Equipment_color = Brushes.LightBlue;
            this.Equipment_state = "Ready";
            this._vIDManager.SetSVID(100, this.Equipment_state);

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
            this._robotArmManager.WaferMoveInfo += Wafer_Position_Draw;

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
            this._secsGemServer.Initialize(AppendLog, this._messageHandler, this._configManager);
            this.IsConnected = true;
            this.IsDisconnected = false;
        }

        [RelayCommand]
        private void Connect()
        {
            this._secsGemServer.Initialize(AppendLog, this._messageHandler, this._configManager);
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

        [RelayCommand]
        private void SubMenuEquipSetting() => NavigateToPage<EquipMenu>();

        [RelayCommand]
        private void SubMenuEventSetting() => NavigateToPage<EventMenu>();
        #endregion

        #region METHODS

        private void OnEquipment_State_Change(object sender, EquipmentStatusEnum state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            { 
                if (state == EquipmentStatusEnum.Running)
                {
                    this.Equipment_color = Brushes.Orange;
                    this.Equipment_state = "Running";
                    this.Setting_menu = false;
                }
                else if (state == EquipmentStatusEnum.Completed)
                {
                    this.Equipment_color = Brushes.LimeGreen;
                    this.Equipment_state = "Completed";
                    this.Setting_menu = true;
                }
                else if (state == EquipmentStatusEnum.Error)
                {
                    this.Equipment_color = Brushes.Red;
                    this.Equipment_state = "Error";
                    this.Setting_menu = true;
                }
                else
                {
                    this.Equipment_color = Brushes.LightBlue;
                    this.Equipment_state = "Ready";
                    this.Setting_menu = true;
                }
            });

            this._vIDManager?.SetSVID(100, this.Equipment_state);
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
            Application.Current.Dispatcher.Invoke(() =>
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
            });
        }

        private void Buffer_DataEnqueued(object? sender, BufferStatus e)
        {
            Application.Current.Dispatcher.Invoke(() =>
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
            });
        }

        private void Wafer_Position_Draw(object? sender, Wafer e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var existingWafer = this.Animation_wafers.FirstOrDefault(w =>
                    w.Wafer_Num == e.Wafer_Num && w.CarrierId == e.CarrierId
                    && w.LoadportId == e.LoadportId);

                if (existingWafer != null)
                {
                    if (!double.IsNaN(e.PositionX) && !double.IsNaN(e.PositionY))
                    {
                        existingWafer.PositionX = locationPositions[e.CurrentLocation].X;
                        existingWafer.PositionY = locationPositions[e.CurrentLocation].Y;

                        if (e.CurrentLocation == "LoadPort1" || e.CurrentLocation == "LoadPort2")
                        {
                            this.Animation_wafers.Remove(existingWafer);
                        }
                    }
                }
                else
                {
                    this.Animation_wafers.Add(new Wafer
                    {
                        LoadportId = e.LoadportId,
                        Wafer_Num = e.Wafer_Num,
                        CarrierId = e.CarrierId ?? "",
                        SlotId = e.SlotId ?? "",
                        PositionX = locationPositions[e.CurrentLocation].X,
                        PositionY = locationPositions[e.CurrentLocation].Y
                    });
                }
            });
        }
        #endregion
    }
}
