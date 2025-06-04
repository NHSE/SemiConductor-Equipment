using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class Buffer2_ViewModel : ObservableObject
    {
        #region FIELDS
        private bool _isInitialized = false;
        private readonly IDatabase<Chamberlogtable>? _database;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private IEnumerable<Chamberlogtable>? _logpagetable;
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public Buffer2_ViewModel(IDatabase<Chamberlogtable> database)
        {
            this._database = database;
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
                this.Logpagetable = await Task.Run(() => this._database?.Search("B2"));
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        #endregion
    }
}
