using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using Wpf.Ui.Abstractions.Controls;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class Buffer_ViewModel : ObservableObject
    {
        #region FIELDS
        private bool _isInitialized = false;
        private readonly IDatabase<Chamberlogtable>? _database;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private IEnumerable<Chamberlogtable>? _logpagetable;
        [ObservableProperty]
        private string? _imagepath;

        #endregion

        #region CONSTRUCTOR
        public Buffer_ViewModel(IDatabase<Chamberlogtable> database)
        {
            this._database = database;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD

        public async Task OnNavigatedToAsync(int? number)
        {
            if (!_isInitialized)
                await InitializeViewModelAsync(number);
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;
        private async Task InitializeViewModelAsync(int? number)
        {
            try
            {
                this.Logpagetable = await Task.Run(() => this._database?.Search($"Buffer{number}"));
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        #endregion
    }
}
