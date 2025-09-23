using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using Wpf.Ui.Abstractions.Controls;
using System.IO;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class CleanChamber6_ViewModel : ObservableObject
    {
        #region FIELDS
        private readonly ICleanManager _cleanManager;
        private readonly ISolutionManager _solutionManager;
        private readonly IEquipmentConfigManager _equipmentConfigManager;
        private readonly ILogManager _logManager;
        private bool _isInitialized = false;
        private FileSystemWatcher _logFileWatcher;
        private long lastLogPosition = 0;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private IEnumerable<Alarmlogtable>? _logpagetable;
        [ObservableProperty]
        private int _isWafer = 0;
        [ObservableProperty]
        private int _chemical = 0;
        [ObservableProperty]
        private int _preClean = 0;
        [ObservableProperty]
        private string _slotID;
        [ObservableProperty]
        private string _state;
        [ObservableProperty]
        private string _cJID;
        [ObservableProperty]
        private string _pJID;
        [ObservableProperty]
        private string _loadPort;
        [ObservableProperty]
        private string _carrierID;
        [ObservableProperty]
        private int _setting_Flow_Rate;
        [ObservableProperty]
        private int _setting_Spray_Time;
        [ObservableProperty]
        private int _setting_PreClean_Flow_Rate;
        [ObservableProperty]
        private int _setting_PreClean_Spray_Time;
        [ObservableProperty]
        private int _setting_RPM;
        [ObservableProperty]
        private string? _logText;

        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Clean Chamber 6 클래스
        /// </summary>
        /// <param name="cleanManager"></param>
        /// <param name="solutionManager"></param>
        /// <param name="equipmentConfigManager"></param>
        /// <param name="logManager"></param>
        public CleanChamber6_ViewModel(ICleanManager cleanManager, ISolutionManager solutionManager, IEquipmentConfigManager equipmentConfigManager, ILogManager logManager)
        {
            this._cleanManager = cleanManager;
            this._solutionManager = solutionManager;
            this._equipmentConfigManager = equipmentConfigManager;
            this._logManager = logManager;

            this._cleanManager.DataEnqueued += CleanManager_DataEnqueued;
            this._cleanManager.MultiCupChange += CleanManager_MultiCupChange;
            this._cleanManager.ChemicalChange += CleanManager_ChemicalChange;
            this._cleanManager.CleanChamberChange += CleanManager_CleanChamberChange;
            this._cleanManager.PreCleanChange += CleanManager_PreCleanChange;

            this._logManager.Subscribe($"Clean_Chamber6", OnLogUpdated);

            LoadInitialLogs();
            SetupLogFileWatcher();
        }

        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        /// <summary>
        /// Log 파일 업데이트 시 변수 저장 메서드
        /// </summary>
        /// <param name="newLog"></param>
        private void OnLogUpdated(string newLog)
        {
            // UI 스레드에서 속성 갱신
            App.Current.Dispatcher.Invoke(() => this.LogText = newLog);
        }

        /// <summary>
        /// Log 파일 읽기 설정 메서드
        /// </summary>
        private void SetupLogFileWatcher()
        {
            var logDirectory = @"C:\Logs";
            var logFileName = $"Clean_Chamber6_{DateTime.Now:yyyyMMdd}_{DateTime.Now:HHmmss}.log";

            _logFileWatcher = new FileSystemWatcher
            {
                Path = logDirectory,
                Filter = logFileName,
                NotifyFilter = NotifyFilters.LastWrite
            };

            _logFileWatcher.Changed += OnLogFileChanged;
            _logFileWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Log 파일 읽기 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLogFileChanged(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    using (var fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        fs.Seek(lastLogPosition, SeekOrigin.Begin);
                        using (var reader = new StreamReader(fs))
                        {
                            string newText = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(newText))
                            {
                                LogText += newText;
                            }
                            lastLogPosition = fs.Position;
                        }
                    }
                }
                catch (IOException)
                {
                    // 파일이 잠겨있을 수 있으니 예외 무시 또는 재시도 로직 추가 가능
                }
            });
        }

        /// <summary>
        /// Log 파일 설정 메서드
        /// </summary>
        private void LoadInitialLogs()
        {
            var logPath = Path.Combine(@"C:\Logs", $"Clean_Chamber6_{DateTime.Now:yyyyMMdd}_{DateTime.Now:HHmmss}.log");
            if (File.Exists(logPath))
            {
                LogText = File.ReadAllText(logPath);
            }
        }

        /// <summary>
        /// Clean 공정에 필요한 데이터 저장 (Equipment Setting 값)
        /// </summary>
        public void Load_Chemical()
        {
            this.Chemical = this._solutionManager.GetValue("Chamber6");
            this.PreClean = this._solutionManager.GetPreCleanValue("Chamber6");
            this.Setting_Spray_Time = this._equipmentConfigManager.Spray_Time;
            this.Setting_RPM = this._equipmentConfigManager.Clean_RPM;
            this.Setting_Flow_Rate = this._equipmentConfigManager.Flow_Rate;
            this.Setting_PreClean_Flow_Rate = this._equipmentConfigManager.PreClean_Flow_Rate;
            this.Setting_PreClean_Spray_Time = this._equipmentConfigManager.PreClean_Spray_Time;
        }

        /// <summary>
        /// Clean 챔버 내 웨이퍼 삽입/반출 시 이벤트 발생에 대한 동작 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cleanChamber"></param>
        private void CleanManager_DataEnqueued(object? sender, CleanChamberStatus cleanChamber)
        {
            if (cleanChamber.ChamberName == "Chamber6")
            {
                this.IsWafer = (this.IsWafer + 1) % 2;
                if(cleanChamber.State != "Running")
                {
                    this.CarrierID = "";
                    this.SlotID = "";
                    this.CJID = "";
                    this.PJID = "";
                    this.LoadPort = "";
                    this.State = "";
                }
            }
        }

        /// <summary>
        /// 사용 X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cleanChamber"></param>
        private void CleanManager_MultiCupChange(object? sender, CleanChamberStatus cleanChamber)
        {
            if (cleanChamber.ChamberName == "Chamber6")
            {
                if (this.IsWafer == 0)
                {
                    this.IsWafer = 1;
                }
                else
                {
                    this.IsWafer = 3 - this.IsWafer;
                }
            }
        }

        /// <summary>
        /// 챔버 내 웨이퍼 삽입 시 정보 저장 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cleanChamber"></param>
        private void CleanManager_CleanChamberChange(object? sender, ChamberData cleanChamber)
        {
            if (cleanChamber.ChamberName == "Chamber6")
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    this.CarrierID = cleanChamber.wafer.CarrierId;
                    this.SlotID = cleanChamber.wafer.SlotId;
                    this.CJID = cleanChamber.wafer.CJId;
                    this.PJID = cleanChamber.wafer.PJId;
                    this.LoadPort = cleanChamber.wafer.LoadportId.ToString();
                    this.State = cleanChamber.wafer.Status;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.CarrierID = cleanChamber.wafer.CarrierId;
                        this.SlotID = cleanChamber.wafer.SlotId;
                        this.CJID = cleanChamber.wafer.CJId;
                        this.PJID = cleanChamber.wafer.PJId;
                        this.LoadPort = cleanChamber.wafer.LoadportId.ToString();
                        this.State = cleanChamber.wafer.Status;
                    });
                }
            }
        }

        /// <summary>
        /// Clean 공정 진행 시 Chemical 값 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cleanChamber"></param>
        private void CleanManager_ChemicalChange(object? sender, ChemicalStatus cleanChamber)
        {
            if (cleanChamber.ChamberName == "Chamber6")
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    if (this._solutionManager.ConsumeChemical(cleanChamber.ChamberName, cleanChamber.Solution))
                    {
                        cleanChamber.Result = false;
                        this.Chemical = this._solutionManager.GetValue(cleanChamber.ChamberName);
                    }
                    else
                    {
                        cleanChamber.Result = true;
                        this.Chemical = this._solutionManager.GetValue(cleanChamber.ChamberName);

                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (this._solutionManager.ConsumeChemical(cleanChamber.ChamberName, cleanChamber.Solution))
                        {
                            cleanChamber.Result = false;
                            this.Chemical = this._solutionManager.GetValue(cleanChamber.ChamberName);
                        }
                        else
                        {
                            cleanChamber.Result = true;
                            this.Chemical = this._solutionManager.GetValue(cleanChamber.ChamberName);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Clean 공정 진행 시 PreClean 값 변경 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cleanChamber"></param>
        private void CleanManager_PreCleanChange(object? sender, ChemicalStatus cleanChamber)
        {
            if (cleanChamber.ChamberName == "Chamber6")
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    if (this._solutionManager.ConsumePreClean(cleanChamber.ChamberName, cleanChamber.Solution))
                    {
                        cleanChamber.Result = false;
                        this.PreClean = this._solutionManager.GetPreCleanValue(cleanChamber.ChamberName);
                    }
                    else
                    {
                        cleanChamber.Result = true;
                        this.PreClean = this._solutionManager.GetPreCleanValue(cleanChamber.ChamberName);

                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (this._solutionManager.ConsumePreClean(cleanChamber.ChamberName, cleanChamber.Solution))
                        {
                            cleanChamber.Result = false;
                            this.PreClean = this._solutionManager.GetPreCleanValue(cleanChamber.ChamberName);
                        }
                        else
                        {
                            cleanChamber.Result = true;
                            this.PreClean = this._solutionManager.GetPreCleanValue(cleanChamber.ChamberName);
                        }
                    });
                }
            }
        }
        #endregion
    }
}
