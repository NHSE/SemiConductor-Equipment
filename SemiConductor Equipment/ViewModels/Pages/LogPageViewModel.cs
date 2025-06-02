using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using System.Windows.Threading;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class LogPageViewModel : ObservableObject
    {
        #region FIELDS
        private bool _isInitialized = false;
        private readonly IDatabase<Chamberlogtable>? _database;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private IEnumerable<Chamberlogtable>? _logpagetable;

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
        #endregion

        #region COMMAND
        [RelayCommand]
        public void OnSearch()
        {
            if (this.SelectChamberName != null)
            {
                var data = this._database?.Search(this.SelectChamberName);
                if(data != null)
                {
                    this.Logpagetable = data;
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        #endregion

        #region METHOD

        public LogPageViewModel(IDatabase<Chamberlogtable> database)
        {
            this._database = database;
            this.Chambername = new List<string>{ "ALL", "LP1", "LP2", "CH1", "CH2", "CH3", "CH4", "CH5", "CH6", "B1", "B2", "B3", "B4"};
        }

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
                this.Logpagetable = await Task.Run(() => _database.Get());
                if (this.Logpagetable != null)
                {
                    //this.Chambername = this.Logpagetable?.Select(x => x.ChamberName).ToList(); // string으로 적어야함
                    //this.Savelogtime = this.Logpagetable?.Select(x => x.Time).Take(3).ToList();

                }
                _isInitialized = true;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
