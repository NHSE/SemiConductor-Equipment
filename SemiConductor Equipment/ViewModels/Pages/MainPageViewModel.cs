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
        private readonly ICleanManager _cleanManager;
        private readonly ISolutionManager _chemicalManager;
        private readonly IRobotArmManager _robotArmManager;
        private readonly IAlarmMsgManager _alarmMsgManager;
        private readonly IMessageManager _messageHandler;
        private readonly IRunningStateManger _runningStateManager;
        private readonly IVIDManager _vIDManager;
        private readonly IPLCManager _plcManager;

        private readonly DispatcherTimer _timer;
        public Dictionary<string, Point> locationPositions = new();
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private string? _currenttime;
        [ObservableProperty]
        private string? _secsdatalog;

        [ObservableProperty]
        private string? _alarmMessage;

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
        private string? _dry_chamber1_state;
        [ObservableProperty]
        private Brush? _dry_chamber1_color;
        [ObservableProperty]
        private string? _dry_chamber2_state;
        [ObservableProperty]
        private Brush? _dry_chamber2_color;
        [ObservableProperty]
        private string? _dry_chamber3_state;
        [ObservableProperty]
        private Brush? _dry_chamber3_color;
        [ObservableProperty]
        private string? _dry_chamber4_state;
        [ObservableProperty]
        private Brush? _dry_chamber4_color;
        [ObservableProperty]
        private string? _dry_chamber5_state;
        [ObservableProperty]
        private Brush? _dry_chamber5_color;
        [ObservableProperty]
        private string? _dry_chamber6_state;
        [ObservableProperty]
        private Brush? _dry_chamber6_color;

        [ObservableProperty]
        private string? _clean_chamber1_state;
        [ObservableProperty]
        private Brush? _clean_chamber1_color;
        [ObservableProperty]
        private string? _clean_chamber2_state;
        [ObservableProperty]
        private Brush? _clean_chamber2_color;
        [ObservableProperty]
        private string? _clean_chamber3_state;
        [ObservableProperty]
        private Brush? _clean_chamber3_color;
        [ObservableProperty]
        private string? _clean_chamber4_state;
        [ObservableProperty]
        private Brush? _clean_chamber4_color;
        [ObservableProperty]
        private string? _clean_chamber5_state;
        [ObservableProperty]
        private Brush? _clean_chamber5_color;
        [ObservableProperty]
        private string? _clean_chamber6_state;
        [ObservableProperty]
        private Brush? _clean_chamber6_color;

        [ObservableProperty]
        private bool? _setting_menu = true;

        [ObservableProperty]
        private ObservableCollection<Wafer> _animation_wafers = new();

        [ObservableProperty]
        private string? _errorlog;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// MainPage 클래스
        /// </summary>
        /// <param name="iDateTime"></param>
        /// <param name="logmanager"></param>
        /// <param name="configManager"></param>
        /// <param name="secsGemServer"></param>
        /// <param name="messageHandler"></param>
        /// <param name="runningStateManager"></param>
        /// <param name="chamberManager"></param>
        /// <param name="cleanManager"></param>
        /// <param name="robotArmManager"></param>
        /// <param name="svIDManager"></param>
        /// <param name="chemicalManager"></param>
        /// <param name="alarmMsgManager"></param>
        /// <param name="pLCManager"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MainPageViewModel(IDateTime iDateTime, ILogManager logmanager, IConfigManager configManager,
            ISecsGemServer secsGemServer, IMessageManager messageHandler, IRunningStateManger runningStateManager, 
            IChamberManager chamberManager, ICleanManager cleanManager, IRobotArmManager robotArmManager, IVIDManager svIDManager
            , ISolutionManager chemicalManager, IAlarmMsgManager alarmMsgManager, IPLCManager plcManager)
        {
            _iDateTime = iDateTime ?? throw new ArgumentNullException(nameof(iDateTime));

            // 초기값
            this.Currenttime = _iDateTime.GetCurrentTime()?.ToString();

            // 타이머 설정
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => this.Currenttime = _iDateTime.GetCurrentTime()?.ToString();
            _timer.Start();

            this._runningStateManager = runningStateManager;
            this._runningStateManager.DataChange += OnEquipment_State_Change;

            this._logmanager = logmanager;
            this._messageHandler = messageHandler;
            this._configManager = configManager;
            this._secsGemServer = secsGemServer;
            this._chamberManager = chamberManager;
            this._cleanManager = cleanManager;
            this._robotArmManager = robotArmManager;
            this._vIDManager = svIDManager;
            this._chemicalManager = chemicalManager;
            this._alarmMsgManager = alarmMsgManager;
            this._plcManager = plcManager;

            this.Equipment_color = Brushes.LightBlue;
            this.Equipment_state = "Ready";
            this._vIDManager.SetSVID(100, this.Equipment_state);

            this._chamberManager.DataEnqueued += Dry_DataEnqueued;
            this._cleanManager.DataEnqueued += Clean_DataEnqueued;
            this._robotArmManager.WaferMoveInfo += Wafer_Position_Draw;
            this._alarmMsgManager.AlarmData += AlarmMsgManager_AlarmData;

            if (this._secsGemServer.Initialize(AppendLog, messageHandler, _configManager))
            {
                this.IsDisconnected = true;
            }

            this._plcManager.Initalize();

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
        /// <summary>
        /// TCP/IP 연결 해제 커맨드
        /// </summary>
        [RelayCommand]
        private void DisConnect()
        {
            this._secsGemServer.Initialize(AppendLog, this._messageHandler, this._configManager);
            this.IsConnected = true;
            this.IsDisconnected = false;
        }

        /// <summary>
        /// TCP/IP 연결 커맨드
        /// </summary>
        [RelayCommand]
        private void Connect()
        {
            this._secsGemServer.Initialize(AppendLog, this._messageHandler, this._configManager);
            this.IsConnected = false;
            this.IsDisconnected = true;
        }

        /// <summary>
        /// LP1로 페이지를 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void LoadPort1() => NavigateToPage<LoadPort1_Page>();

        /// <summary>
        /// LP2로 페이지를 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void LoadPort2() => NavigateToPage<LoadPort2_Page>();

        /// <summary>
        /// Clean Chamber 1 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void CleanChamber1() => NavigateToPage<CleanChamber1_Page>();

        /// <summary>
        /// Clean Chamber 2 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void CleanChamber2() => NavigateToPage<CleanChamber2_Page>();

        /// <summary>
        /// Clean Chamber 3 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void CleanChamber3() => NavigateToPage<CleanChamber3_Page>();

        /// <summary>
        /// Clean Chamber 4 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void CleanChamber4() => NavigateToPage<CleanChamber4_Page>();

        /// <summary>
        /// Clean Chamber 5 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void CleanChamber5() => NavigateToPage<CleanChamber5_Page>();

        /// <summary>
        /// Clean Chamber 6 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void CleanChamber6() => NavigateToPage<CleanChamber6_Page>();

        /// <summary>
        /// Dry Chamber 1 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void DryChamber1() => NavigateToPage<Chamber1_Page>();

        /// <summary>
        /// Dry Chamber 2 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void DryChamber2() => NavigateToPage<Chamber2_Page>();

        /// <summary>
        /// Dry Chamber 3 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void DryChamber3() => NavigateToPage<Chamber3_Page>();

        /// <summary>
        /// Dry Chamber 4 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void DryChamber4() => NavigateToPage<Chamber4_Page>();

        /// <summary>
        /// Dry Chamber 5 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void DryChamber5() => NavigateToPage<Chamber5_Page>();

        /// <summary>
        /// Dry Chamber 6 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void DryChamber6() => NavigateToPage<Chamber6_Page>();

        /// <summary>
        /// IP Setting 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void SubMenuIPSetting() => NavigateToPage<IpSettingMenu>();

        /// <summary>
        /// Solution Setting 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void SubMenuChemicalSetting() => NavigateToPage<SolutionSettingMenu>();

        /// <summary>
        /// Equipment Setting 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void SubMenuEquipSetting() => NavigateToPage<EquipMenu>();

        /// <summary>
        /// Event Setting 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void SubMenuEventSetting() => NavigateToPage<EventMenu>();

        /// <summary>
        /// Alarm Moniter 페이지로 바꾸는 커맨드
        /// </summary>
        [RelayCommand]
        private void SubMenuAlarmMonitor() => NavigateToPage<AlarmLogMenu>();
        #endregion

        #region METHODS
        /// <summary>
        /// 공정 상태에 따라 버튼, State 값 변경 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        private void OnEquipment_State_Change(object sender, EquipmentStatusEnum state)
        {
            if (Application.Current.Dispatcher.CheckAccess())
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
            }
            else
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
            }

            this._vIDManager?.SetSVID(100, this.Equipment_state);
        }

        /// <summary>
        /// Page 변경 메서드
        /// </summary>
        /// <typeparam name="TPage"></typeparam>
        private void NavigateToPage<TPage>() where TPage : class
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var page = App.Services.GetRequiredService<TPage>();
                mainWindow.MainFrame.Navigate(page);
            }
        }

        /// <summary>
        /// 알람 이벤트 발생 시 메세지 출력하는 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlarmMsgManager_AlarmData(object? sender, string e)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                this.AlarmMessage = e;
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.AlarmMessage = e;
                });
            }
        }

        /// <summary>
        /// Clean Chamber 내 Solution 의 유무를 통해 DISABLE 설정하는 메서드
        /// </summary>
        private void Check_CleanChamber_Enable()
        {
            for(int chamber_num = 1; chamber_num < 7; chamber_num++)
            {
                string chambername = "Chamber" + chamber_num;
                if(this._chemicalManager.GetValue(chambername) == 0)
                {
                    this._cleanManager.Unable_to_Process[chambername] = true;
                    this._cleanManager.Clean_State[chambername] = "DISAB";
                }
                else if(this._chemicalManager.GetPreCleanValue(chambername) == 0)
                {
                    this._cleanManager.Unable_to_Process[chambername] = true;
                    this._cleanManager.Clean_State[chambername] = "DISAB";
                }
                else
                {
                    if (this._cleanManager.CleanChamberEmpty(chambername))
                    {
                        this._cleanManager.Clean_State[chambername] = "IDLE";
                    }
                    this._cleanManager.Unable_to_Process[chambername] = false;
                }
            }
        }

        /// <summary>
        /// 상태에 따라 챔버 밑 텍스트의 색을 변경하는 메서드
        /// </summary>
        /// <param name="Draw_Type"></param>
        public void Draw_Color(string Draw_Type)
        {
            if (Draw_Type == "Chamber")
                Draw_DryChamber_Color();
            else if (Draw_Type == "Clean")
                Draw_CleanChamber_Color();
            else
            {
                Draw_DryChamber_Color();
                Draw_CleanChamber_Color();
            }
        }

        /// <summary>
        /// 각 챔버의 상태 획득 메서드
        /// </summary>
        public void Get_Chamber_State()
        {
            this.Dry_chamber1_state = this._chamberManager.Chamber_State["Chamber1"];
            this.Dry_chamber2_state = this._chamberManager.Chamber_State["Chamber2"];
            this.Dry_chamber3_state = this._chamberManager.Chamber_State["Chamber3"];
            this.Dry_chamber4_state = this._chamberManager.Chamber_State["Chamber4"];
            this.Dry_chamber5_state = this._chamberManager.Chamber_State["Chamber5"];
            this.Dry_chamber6_state = this._chamberManager.Chamber_State["Chamber6"];

            Check_CleanChamber_Enable();
            this.Clean_chamber1_state = this._cleanManager.Clean_State["Chamber1"];
            this.Clean_chamber2_state = this._cleanManager.Clean_State["Chamber2"];
            this.Clean_chamber3_state = this._cleanManager.Clean_State["Chamber3"];
            this.Clean_chamber4_state = this._cleanManager.Clean_State["Chamber4"];
            this.Clean_chamber5_state = this._cleanManager.Clean_State["Chamber5"];
            this.Clean_chamber6_state = this._cleanManager.Clean_State["Chamber6"];
        }

        /// <summary>
        /// 각 Clean 챔버의 상태에 따라 상태 텍스트 색 설정 메서드
        /// </summary>
        private void Draw_CleanChamber_Color()
        {
            if (this.Clean_chamber1_state == "IDLE")
                this.Clean_chamber1_color = Brushes.DarkGray;
            else if (this.Clean_chamber1_state == "DONE")
                this.Clean_chamber1_color = Brushes.LightGreen;
            else if (this.Clean_chamber1_state == "Running")
                this.Clean_chamber1_color = Brushes.DarkOrange;
            else
                this.Clean_chamber1_color = Brushes.Red;

            if (this.Clean_chamber2_state == "IDLE")
                this.Clean_chamber2_color = Brushes.DarkGray;
            else if (this.Clean_chamber2_state == "DONE")
                this.Clean_chamber2_color = Brushes.LightGreen;
            else if (this.Clean_chamber2_state == "Running")
                this.Clean_chamber2_color = Brushes.DarkOrange;
            else
                this.Clean_chamber2_color = Brushes.Red;

            if (this.Clean_chamber3_state == "IDLE")
                this.Clean_chamber3_color = Brushes.DarkGray;
            else if (this.Clean_chamber3_state == "DONE")
                this.Clean_chamber3_color = Brushes.LightGreen;
            else if (this.Clean_chamber3_state == "Running")
                this.Clean_chamber3_color = Brushes.DarkOrange;
            else
                this.Clean_chamber3_color = Brushes.Red;

            if (this.Clean_chamber4_state == "IDLE")
                this.Clean_chamber4_color = Brushes.DarkGray;
            else if (this.Clean_chamber4_state == "DONE")
                this.Clean_chamber4_color = Brushes.LightGreen;
            else if (this.Clean_chamber4_state == "Running")
                this.Clean_chamber4_color = Brushes.DarkOrange;
            else
                this.Clean_chamber4_color = Brushes.Red;

            if (this.Clean_chamber5_state == "IDLE")
                this.Clean_chamber5_color = Brushes.DarkGray;
            else if (this.Clean_chamber5_state == "DONE")
                this.Clean_chamber5_color = Brushes.LightGreen;
            else if (this.Clean_chamber5_state == "Running")
                this.Clean_chamber5_color = Brushes.DarkOrange;
            else
                this.Clean_chamber5_color = Brushes.Red;

            if (this.Clean_chamber6_state == "IDLE")
                this.Clean_chamber6_color = Brushes.DarkGray;
            else if (this.Clean_chamber6_state == "DONE")
                this.Clean_chamber6_color = Brushes.LightGreen;
            else if (this.Clean_chamber6_state == "Running")
                this.Clean_chamber6_color = Brushes.DarkOrange;
            else
                this.Clean_chamber6_color = Brushes.Red;
        }

        /// <summary>
        /// 각 Dry 챔버의 상태에 따라 상태 텍스트 색 설정 메서드
        /// </summary>
        private void Draw_DryChamber_Color()
        {
            if (this.Dry_chamber1_state == "IDLE")
                this.Dry_chamber1_color = Brushes.DarkGray;
            else if (this.Dry_chamber1_state == "DONE")
                this.Dry_chamber1_color = Brushes.LightGreen;
            else
                this.Dry_chamber1_color = Brushes.DarkOrange;

            if (this.Dry_chamber2_state == "IDLE")
                this.Dry_chamber2_color = Brushes.DarkGray;
            else if (this.Dry_chamber2_state == "DONE")
                this.Dry_chamber2_color = Brushes.LightGreen;
            else
                this.Dry_chamber2_color = Brushes.DarkOrange;

            if (this.Dry_chamber3_state == "IDLE")
                this.Dry_chamber3_color = Brushes.DarkGray;
            else if (this.Dry_chamber3_state == "DONE")
                this.Dry_chamber3_color = Brushes.LightGreen;
            else
                this.Dry_chamber3_color = Brushes.DarkOrange;

            if (this.Dry_chamber4_state == "IDLE")
                this.Dry_chamber4_color = Brushes.DarkGray;
            else if (this.Dry_chamber4_state == "DONE")
                this.Dry_chamber4_color = Brushes.LightGreen;
            else
                this.Dry_chamber4_color = Brushes.DarkOrange;

            if (this.Dry_chamber5_state == "IDLE")
                this.Dry_chamber5_color = Brushes.DarkGray;
            else if (this.Dry_chamber5_state == "DONE")
                this.Dry_chamber5_color = Brushes.LightGreen;
            else
                this.Dry_chamber5_color = Brushes.DarkOrange;

            if (this.Dry_chamber6_state == "IDLE")
                this.Dry_chamber6_color = Brushes.DarkGray;
            else if (this.Dry_chamber6_state == "DONE")
                this.Dry_chamber6_color = Brushes.LightGreen;
            else
                this.Dry_chamber6_color = Brushes.DarkOrange;
        }

        /// <summary>
        /// SECS/GEM 연결 상태를 텍스트로 보여주는 메서드
        /// </summary>
        /// <param name="text"></param>
        public void AppendLog(string text)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                Secsdatalog += ($"{text}") + "\n";
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Secsdatalog += ($"{text}") + "\n";
                });
            }
        }

        /// <summary>
        /// Dry 챔버 상태 변경 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dry_DataEnqueued(object? sender, ChamberStatus e)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                switch (e.ChamberName)
                {
                    case "Chamber1":
                        this.Dry_chamber1_state = e.State;
                        break;

                    case "Chamber2":
                        this.Dry_chamber2_state = e.State;
                        break;

                    case "Chamber3":
                        this.Dry_chamber3_state = e.State;
                        break;

                    case "Chamber4":
                        this.Dry_chamber4_state = e.State;
                        break;

                    case "Chamber5":
                        this.Dry_chamber5_state = e.State;
                        break;

                    case "Chamber6":
                        this.Dry_chamber6_state = e.State;
                        break;
                }

                Draw_Color("Chamber");
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    switch (e.ChamberName)
                    {
                        case "Chamber1":
                            this.Dry_chamber1_state = e.State;
                            break;

                        case "Chamber2":
                            this.Dry_chamber2_state = e.State;
                            break;

                        case "Chamber3":
                            this.Dry_chamber3_state = e.State;
                            break;

                        case "Chamber4":
                            this.Dry_chamber4_state = e.State;
                            break;

                        case "Chamber5":
                            this.Dry_chamber5_state = e.State;
                            break;

                        case "Chamber6":
                            this.Dry_chamber6_state = e.State;
                            break;
                    }

                    Draw_Color("Chamber");
                });
            }
        }


        /// <summary>
        /// Clean 챔버 상태 변경 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clean_DataEnqueued(object? sender, CleanChamberStatus e)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                switch (e.ChamberName)
                {
                    case "Chamber1":
                        this.Clean_chamber1_state = e.State;
                        break;

                    case "Chamber2":
                        this.Clean_chamber2_state = e.State;
                        break;

                    case "Chamber3":
                        this.Clean_chamber3_state = e.State;
                        break;

                    case "Chamber4":
                        this.Clean_chamber4_state = e.State;
                        break;

                    case "Chamber5":
                        this.Clean_chamber5_state = e.State;
                        break;

                    case "Chamber6":
                        this.Clean_chamber6_state = e.State;
                        break;
                }
                Draw_Color("Clean");
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    switch (e.ChamberName)
                    {
                        case "Chamber1":
                            this.Clean_chamber1_state = e.State;
                            break;

                        case "Chamber2":
                            this.Clean_chamber2_state = e.State;
                            break;

                        case "Chamber3":
                            this.Clean_chamber3_state = e.State;
                            break;

                        case "Chamber4":
                            this.Clean_chamber4_state = e.State;
                            break;

                        case "Chamber5":
                            this.Clean_chamber5_state = e.State;
                            break;

                        case "Chamber6":
                            this.Clean_chamber6_state = e.State;
                            break;
                    }
                    Draw_Color("Clean");
                });
            }
        }

        /// <summary>
        /// 웨이퍼의 위치에 따라 원을 위치시켜주는 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Wafer_Position_Draw(object? sender, Wafer e)
        {
            if (Application.Current.Dispatcher.CheckAccess())
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
            }
            else
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
        }
        #endregion
    }
}
