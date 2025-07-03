using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using System.Windows.Threading;
using SemiConductor_Equipment.Dtos;

namespace SemiConductor_Equipment.ViewModels.Menus
{
    public partial class LogPageViewModel : ObservableObject
    {
        #region FIELDS
        private bool _isInitialized = false;
        private readonly IDatabase<Chamberlogtable>? _database;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private IEnumerable<ChamberlogDisplayDto>? _logpagetable;

        [ObservableProperty]
        private List<string>? _chambername;

        [ObservableProperty]
        private IEnumerable<string>? _savelogtime;

        [ObservableProperty]
        private IEnumerable<string>? _selectlogdata;

        [ObservableProperty]
        private string? _selectChamberName = "ALL";

        #endregion

        #region CONSTRUCTOR
        public LogPageViewModel(IDatabase<Chamberlogtable> database)
        {
            this._database = database;
            this.Chambername = new List<string> { "ALL", "Loadport1", "Loadport2", "Chamber1", "Chamber2", "Chamber3", 
                                                "Chamber4", "Chamber5", "Chamber6", "Buffer1", "Buffer2", "Buffer3", "Buffer4" };
        }
        #endregion

        #region COMMAND
        [RelayCommand]
        public void OnSearch()
        {
            if (this.SelectChamberName != null)
            {
                var data = this._database?.Search(this.SelectChamberName);
                if (data != null)
                {
                    this.Logpagetable = data.Select(d => new ChamberlogDisplayDto
                    {
                        ChamberName = d.ChamberName,
                        Time = d.Time,
                        WaferId = d.WaferId,
                        Slot = d.Slot,
                        Logdata = d.Logdata,
                        LotId = d.LotId,
                        State = d.State,
                        
                    });
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        #endregion

        #region METHOD

        public async Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                await InitializeViewModelAsync();
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;
        private async Task InitializeViewModelAsync()
        {
            try
            {
                var logList = await Task.Run(() => _database.Get());

                // DTO로 변환
                this.Logpagetable = logList.Select(log => new ChamberlogDisplayDto
                {
                    ChamberName = log.ChamberName,
                    Time = log.Time,
                    WaferId = log.WaferId,
                    Slot = log.Slot,
                    Logdata = log.Logdata,
                    LotId = log.LotId,
                    State = log.State,
                    // 필요한 필드 계속 매핑
                }).ToList();

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                throw new Exception("로그 초기화 실패", ex);
            }
        }
        #endregion
    }
}
