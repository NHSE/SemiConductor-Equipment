using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Dtos;
using SemiConductor_Equipment.Helpers;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.ViewModels.Windows
{
    public partial class AlarmLogHistoryViewModel : ObservableObject
    {
        #region FIELDS
        private readonly DbLogHelper _logHelper;
        private readonly IMessageBox _messageBoxManager;
        private readonly IDatabase<Alarmlogtable>? _database;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private IEnumerable<AlarmlogDisplayDto>? _logpagetable;
        #endregion

        #region CONSTRUCTOR
        public AlarmLogHistoryViewModel(IDatabase<Alarmlogtable> database, IMessageBox messageBoxManager)
        {
            this._database = database;
            this._messageBoxManager = messageBoxManager;
        }
        #endregion

        #region COMMAND

        #endregion

        #region METHOD
        public async Task OnNavigatedToAsync()
        {
            await InitializeViewModelAsync();
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;
        private async Task InitializeViewModelAsync()
        {
            try
            {
                if (_database is null)
                    throw new InvalidOperationException();

                var logList = await Task.Run(() => _database.Get());

                // DTO로 변환
                this.Logpagetable = logList.Select(log => new AlarmlogDisplayDto
                {
                    Alarm_Number = log.AlarmNumber,
                    Alarm_Time = log.AlarmTime,
                    Alarm_Message = log.AlarmMessage,
                    // 필요한 필드 계속 매핑
                }).ToList();
            }
            catch (Exception ex)
            {
                this._messageBoxManager.Show("예외 발생!", "DataBase에 연결되지 않았습니다.");
            }
        }
        #endregion
    }
}
