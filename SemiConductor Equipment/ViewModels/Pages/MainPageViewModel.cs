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
        private readonly LogService _logService;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private string? _currenttime;

        public Chamber_ViewModel Chamber1 { get; }
        public Chamber_ViewModel Chamber2 { get; }
        public Chamber_ViewModel Chamber3 { get; }
        public Chamber_ViewModel Chamber4 { get; }
        public Chamber_ViewModel Chamber5 { get; }
        public Chamber_ViewModel Chamber6 { get; }
        #endregion

        #region CONSTRUCTOR
        public MainPageViewModel(IDateTime iDateTime, LogService logService)
        {
            _iDateTime = iDateTime ?? throw new ArgumentNullException(nameof(iDateTime));

            // 초기값
            this.Currenttime = _iDateTime.GetCurrentTime()?.ToString();

            // 타이머 설정
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => this.Currenttime = _iDateTime.GetCurrentTime()?.ToString();
            _timer.Start();

            _logService = logService;

            //뷰모델 등록
            var chamberService = App.Services.GetService<IChamberService>();
            var messageBox = App.Services.GetService<IMessageBox>();

            var db = new LogDatabaseContext(); // 실제 DB 컨텍스트
            IDatabase<ChamberStatus> database = new ChamberStatusService(db);

            Chamber1 = new Chamber_ViewModel(database, logService, chamberService, messageBox, 1);
            Chamber2 = new Chamber_ViewModel(database, logService, chamberService, messageBox, 2);
            Chamber3 = new Chamber_ViewModel(database, logService, chamberService, messageBox, 3);
            Chamber4 = new Chamber_ViewModel(database, logService, chamberService, messageBox, 4);
            Chamber5 = new Chamber_ViewModel(database, logService, chamberService, messageBox, 5);
            Chamber6 = new Chamber_ViewModel(database, logService, chamberService, messageBox, 6);
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

        #endregion
    }
}
