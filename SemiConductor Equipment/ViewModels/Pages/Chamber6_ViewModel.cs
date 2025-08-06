using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading;
using SemiConductor_Equipment.Services;
using System.Timers;
using System.Windows.Threading;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Enums;
using CommunityToolkit.Mvvm.Messaging;
using SemiConductor_Equipment.Messages;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SemiConductor_Equipment.Models;
using System.Data;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Defaults;
using System.Windows.Data;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class Chamber6_ViewModel : ObservableObject
    {
        #region FIELDS
        private readonly ILogManager _logManager;
        private readonly IChamberManager _chamberManager;
        private readonly IEquipmentConfigManager _equipmentConfigManager;
        private FileSystemWatcher _logFileWatcher;
        private readonly object _itemLock1 = new object();
        private long lastLogPosition = 0;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private string? _logText;

        [ObservableProperty]
        private string? _statusText;

        [ObservableProperty]
        private bool _isReadyToRun;

        [ObservableProperty]
        private bool hasWafer;

        [ObservableProperty]
        private List<string>? _logpagetable;

        [ObservableProperty]
        private bool _isWafer;

        [ObservableProperty]
        private int _waferNumber;

        [ObservableProperty]
        private Dictionary<int, ObservableCollection<ObservablePoint>> waferDataDict = new Dictionary<int, ObservableCollection<ObservablePoint>>();

        [ObservableProperty]
        private ObservableCollection<ISeries> _series = new ObservableCollection<ISeries>();

        [ObservableProperty]
        private Axis[] _yAxes;

        [ObservableProperty]
        private Axis[] _xAxes;

        [ObservableProperty]
        private ObservableCollection<RectangularSection> _sections;

        [ObservableProperty]
        private double _minTemp;

        [ObservableProperty]
        private double _maxTemp;

        [ObservableProperty]
        private double _graphtime;
        #endregion

        #region CONSTRUCTOR
        public Chamber6_ViewModel(ILogManager logService, IMessageBox messageBox, IChamberManager chamberManager, IEquipmentConfigManager equipmentConfigManager)
        {
            this._logManager = logService;
            // 구독: 로그가 갱신될 때마다 OnLogUpdated 호출
            this._logManager.Subscribe($"Chamber6", OnLogUpdated);

            LoadInitialLogs();
            SetupLogFileWatcher();

            this.IsReadyToRun = false;
            this.HasWafer = false;

            this._chamberManager = chamberManager;
            this.StatusText = this._chamberManager.Chamber_State["Chamber6"];
            this._chamberManager.DataEnqueued += OnDataEnqueued;
            this._chamberManager.ChangeTempData += OnTempChanged;
            this._chamberManager.ProcessHandled += OnProcess;

            this._equipmentConfigManager = equipmentConfigManager;
            this._equipmentConfigManager.ConfigRead += ChangeTempData;
            this._equipmentConfigManager.InitConfig();

            BindingOperations.EnableCollectionSynchronization(waferDataDict, _itemLock1);
        }

        #endregion

        #region COMMAND
        #endregion

        #region METHOD

        private void SetupLogFileWatcher()
        {
            var logDirectory = @"C:\Logs";
            var logFileName = $"Chamber6_{DateTime.Now:yyyyMMdd}_{DateTime.Now:HHmmss}.log";

            _logFileWatcher = new FileSystemWatcher
            {
                Path = logDirectory,
                Filter = logFileName,
                NotifyFilter = NotifyFilters.LastWrite
            };

            _logFileWatcher.Changed += OnLogFileChanged;
            _logFileWatcher.EnableRaisingEvents = true;
        }

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

        private void LoadInitialLogs()
        {
            var logPath = Path.Combine(@"C:\Logs", $"Chamber6_{DateTime.Now:yyyyMMdd}_{DateTime.Now:HHmmss}.log");
            if (File.Exists(logPath))
            {
                LogText = File.ReadAllText(logPath);
            }
        }

        private void OnLogUpdated(string newLog)
        {
            // UI 스레드에서 속성 갱신
            App.Current.Dispatcher.Invoke(() => this.LogText = newLog);
        }

        private void OnDataEnqueued(object sender, ChamberStatus chamber)
        {
            // 큐에 데이터가 들어왔을 때 실행할 코드
            // 필요하다면 _service에서 직접 큐 상태를 조회할 수 있음
            //이벤트 넘겨줄때 챔버 네임 넘겨서 받아야함
            //그 이후 각 뷰모델에서 자기 이름과 같으면 bool 반전
            if (chamber.ChamberName == "Chamber6")
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    this.IsWafer = !this.IsWafer;
                    this.StatusText = chamber.State;

                    if (!waferDataDict.ContainsKey(chamber.WaferName))
                    {
                        var newList = new ObservableCollection<ObservablePoint>();
                        BindingOperations.EnableCollectionSynchronization(newList, _itemLock1);
                        waferDataDict[chamber.WaferName] = newList;

                        Series.Add(new LineSeries<ObservablePoint>
                        {
                            Values = waferDataDict[chamber.WaferName],
                            Fill = null,
                            GeometrySize = 0,
                            Stroke = new SolidColorPaint(GetColorByWaferId(chamber.WaferName.ToString()), 2),
                            Name = chamber.WaferName.ToString()
                        });
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.IsWafer = !this.IsWafer;
                        this.StatusText = chamber.State;

                        if (!waferDataDict.ContainsKey(chamber.WaferName))
                        {
                            var newList = new ObservableCollection<ObservablePoint>();
                            BindingOperations.EnableCollectionSynchronization(newList, _itemLock1);
                            waferDataDict[chamber.WaferName] = newList;

                            Series.Add(new LineSeries<ObservablePoint>
                            {
                                Values = waferDataDict[chamber.WaferName],
                                Fill = null,
                                GeometrySize = 0,
                                Stroke = new SolidColorPaint(GetColorByWaferId(chamber.WaferName.ToString()), 2),
                                Name = chamber.WaferName.ToString()
                            });
                        }
                    });
                }
            }
        }

        private SKColor GetColorByWaferId(string waferId)
        {
            // 예시로 해시값 기반 색상 설정
            int hash = waferId.GetHashCode();
            var r = (byte)((hash >> 16) & 0xFF);
            var g = (byte)((hash >> 8) & 0xFF);
            var b = (byte)(hash & 0xFF);

            return new SKColor(r, g, b);
        }

        private void OnProcess()
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                this.waferDataDict.Clear();
                this.Series.Clear();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.waferDataDict.Clear();
                    this.Series.Clear();
                });
            }
        }

        private void OnTempChanged(object? sender, Wafer e)
        {
            if (e.CurrentLocation == "Dry Chamber_Chamber6")
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    lock (_itemLock1)
                    {
                        // X값, Y값은 이벤트 데이터에서 받아서 넣어야 함
                        // 예) X값: e.Time, Y값: e.RequiredTemperature (가정)
                        double xValue = e.RunningTime; // 실제 이벤트 데이터에 맞게 변경
                        double yValue = e.RequiredTemperature; // 필요 시 변환
                        if (!waferDataDict.ContainsKey(e.Wafer_Num))
                        {
                            waferDataDict[e.Wafer_Num] = new ObservableCollection<ObservablePoint>();
                        }
                        waferDataDict[e.Wafer_Num].Add(new ObservablePoint(xValue, yValue));
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        lock (_itemLock1)
                        {
                            // X값, Y값은 이벤트 데이터에서 받아서 넣어야 함
                            // 예) X값: e.Time, Y값: e.RequiredTemperature (가정)
                            double xValue = e.RunningTime; // 실제 이벤트 데이터에 맞게 변경
                            double yValue = e.RequiredTemperature; // 필요 시 변환
                            if (!waferDataDict.ContainsKey(e.Wafer_Num))
                            {
                                waferDataDict[e.Wafer_Num] = new ObservableCollection<ObservablePoint>();
                            }
                            waferDataDict[e.Wafer_Num].Add(new ObservablePoint(xValue, yValue));
                        }
                    });
                }
            }
        }

        private void ChangeTempData()
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                lock (_itemLock1)
                {
                    this.MinTemp = _equipmentConfigManager.Min_Temp;
                    this.MaxTemp = _equipmentConfigManager.Max_Temp;
                    this.Graphtime = _equipmentConfigManager.Chamber_Time;
                    Sections = new ObservableCollection<RectangularSection>
                        {
                            new RectangularSection
                            {
                                Xi = 0,
                                Xj = this.Graphtime, // 0초 ~ 설정 시간까지
                                Yi = this.MinTemp,
                                Yj = this.MinTemp + 1, // 아주 얇은 "수평 영역" (수평선처럼 보임)
                                Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(128)) // 반투명 파랑
                            },
                            new RectangularSection
                            {
                                Xi = 0,
                                Xj = this.Graphtime,
                                Yi = this.MaxTemp,
                                Yj = this.MaxTemp + 1,
                                Fill = new SolidColorPaint(SKColors.Red.WithAlpha(128)) // 반투명 빨강
                            }
                        };

                    // 3. X축 범위 설정 (0초 ~ Time초)
                    XAxes = new Axis[]
                    {
                            new Axis
                            {
                                Labeler = value => TimeSpan.FromSeconds(value).ToString(@"mm\:ss"),
                                MinLimit = 0,
                                MaxLimit = this.Graphtime,
                                Name = "Time",
                                NamePaint = new SolidColorPaint
                                {
                                    Color = SKColors.LightGray,
                                },
                                LabelsPaint = new SolidColorPaint
                                {
                                    Color = SKColors.LightGray,
                                }
                            }
                    };

                    YAxes = new Axis[]
                    {
                            new Axis
                            {
                                MinLimit = 0,
                                Name = "Temp",
                                Labeler = value => $"{value}˚C",
                                NamePaint = new SolidColorPaint
                                {
                                    Color = SKColors.LightGray,
                                },
                                LabelsPaint = new SolidColorPaint
                                {
                                    Color = SKColors.LightGray,
                                }
                            }
                    };
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    lock (_itemLock1)
                    {
                        this.MinTemp = _equipmentConfigManager.Min_Temp;
                        this.MaxTemp = _equipmentConfigManager.Max_Temp;
                        this.Graphtime = _equipmentConfigManager.Chamber_Time;
                        Sections = new ObservableCollection<RectangularSection>
                        {
                            new RectangularSection
                            {
                                Xi = 0,
                                Xj = this.Graphtime, // 0초 ~ 설정 시간까지
                                Yi = this.MinTemp,
                                Yj = this.MinTemp + 1, // 아주 얇은 "수평 영역" (수평선처럼 보임)
                                Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(128)) // 반투명 파랑
                            },
                            new RectangularSection
                            {
                                Xi = 0,
                                Xj = this.Graphtime,
                                Yi = this.MaxTemp,
                                Yj = this.MaxTemp + 1,
                                Fill = new SolidColorPaint(SKColors.Red.WithAlpha(128)) // 반투명 빨강
                            }
                        };

                        // 3. X축 범위 설정 (0초 ~ Time초)
                        XAxes = new Axis[]
                        {
                                new Axis
                                {
                                    Labeler = value => TimeSpan.FromSeconds(value).ToString(@"mm\:ss"),
                                    MinLimit = 0,
                                    MaxLimit = this.Graphtime,
                                    Name = "Time",
                                    NamePaint = new SolidColorPaint
                                    {
                                        Color = SKColors.LightGray,
                                        FontFamily = "AppleGothic",
                                    },
                                    LabelsPaint = new SolidColorPaint
                                    {
                                        Color = SKColors.LightGray,
                                        FontFamily = "AppleGothic"
                                    }
                                }
                        };

                        YAxes = new Axis[]
                        {
                            new Axis
                            {
                                MinLimit = 0,
                                Name = "Temp",
                                Labeler = value => $"{value}˚C",
                                NamePaint = new SolidColorPaint
                                {
                                    Color = SKColors.LightGray,
                                    FontFamily = "AppleGothic",
                                },
                                LabelsPaint = new SolidColorPaint
                                {
                                    Color = SKColors.LightGray,
                                    FontFamily = "AppleGothic"
                                }
                            }
                        };
                    }
                });
            }
        }
        #endregion
    }
}
