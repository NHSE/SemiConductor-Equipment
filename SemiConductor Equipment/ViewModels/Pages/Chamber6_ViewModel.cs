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
        private readonly IChamberService _service;
        private readonly IMessageBox _messageBox;
        private readonly IDatabase<ChamberStatus>? _database;
        private FileSystemWatcher _logFileWatcher;
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
        #endregion

        #region CONSTRUCTOR
        public Chamber6_ViewModel(IDatabase<ChamberStatus> database, ILogManager logService, IChamberService service, IMessageBox messageBox)
        {
            this._logManager = logService;
            // 구독: 로그가 갱신될 때마다 OnLogUpdated 호출
            this._logManager.Subscribe($"Chamber6", OnLogUpdated);

            LoadInitialLogs();
            SetupLogFileWatcher();

            this._database = database;

            this._service = service;
            this._messageBox = messageBox;
            this._service.ErrorOccurred += OnErrorOccurred;

            this.IsReadyToRun = false;
            this.HasWafer = false;

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
                LogText = File.ReadAllText(e.FullPath);
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

        private void OnErrorOccurred(string message)
        {
            this._messageBox.Show(message);
        }

        public void PrepareRun(int number)
        {
            IsReadyToRun = true;
            TryStartRun(number);
        }

        // 외부(센서/SECS 등)에서 웨이퍼 감지 시 호출
        public void OnWaferInserted(int number)
        {
            HasWafer = true;
            TryStartRun(number);
        }

        private void TryStartRun(int number)
        {
            if (IsReadyToRun && HasWafer)
            {
                StartRun(number);
                IsReadyToRun = false; // 1회성 실행이라면 해제
            }
        }

        private void StartRun(int number)
        {
            // 실제 챔버 동작 로직
            this._service.RunChamber(number);
        }

        public async Task OnNavigatedToAsync(int? number)
        {
               await InitializeViewModelAsync((number.ToString()));
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;
        private async Task InitializeViewModelAsync(string? number)
        {
            try
            {
                this.Logpagetable = await Task.Run(() => this._database?.SearchChamberField($"ch{number}"));
                this.StatusText = this.Logpagetable?.FirstOrDefault() ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        #endregion
    }
}
