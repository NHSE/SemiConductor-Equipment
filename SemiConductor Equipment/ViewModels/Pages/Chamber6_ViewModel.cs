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

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class Chamber6_ViewModel : ObservableObject
    {
        #region FIELDS
        private readonly ILogManager _logManager;
        private readonly IChamberManager _chamberManager;
        private FileSystemWatcher _logFileWatcher;
        private long lastLogPosition = 0;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private string? _logText;

        [ObservableProperty]
        private string? _statusText;

        [ObservableProperty]
        private bool isReadyToRun;

        [ObservableProperty]
        private bool hasWafer;

        [ObservableProperty]
        private List<string>? _logpagetable;

        [ObservableProperty]
        private bool _isWafer;
        #endregion

        #region CONSTRUCTOR
        public Chamber6_ViewModel(ILogManager logService, IMessageBox messageBox, IChamberManager chamberManager)
        {
            this._logManager = logService;
            // 구독: 로그가 갱신될 때마다 OnLogUpdated 호출
            this._logManager.Subscribe($"Chamber6", OnLogUpdated);

            LoadInitialLogs();
            SetupLogFileWatcher();

            this.IsReadyToRun = false;
            this.HasWafer = false;
            this._chamberManager = chamberManager;

            this._chamberManager.DataEnqueued += OnDataEnqueued;

        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        private void SetupLogFileWatcher()
        {
            var logDirectory = @"C:\Logs";
            var logFileName = $"Chamber6_{DateTime.Now:yyyyMMdd}.log";

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
            var logPath = Path.Combine(@"C:\Logs", $"Chamber6_{DateTime.Now:yyyyMMdd}.log");
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (chamber.ChamberName == "Chamber6")
                {
                    this.IsWafer = !this.IsWafer;
                    this.StatusText = chamber.State;
                }
            });
        }
        #endregion
    }
}
