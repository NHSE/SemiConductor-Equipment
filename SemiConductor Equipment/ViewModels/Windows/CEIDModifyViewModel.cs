using Secs4Net;
using SemiConductor_Equipment.interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.ViewModels.Windows
{
    public partial class CEIDModifyViewModel : ObservableObject
    {
        #region FIELDS
        private CEIDInfo _item;
        private readonly IEventConfigManager _configManager;
        public event Action CloseRequested;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private ObservableCollection<int> allSvids = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        [ObservableProperty]
        private ObservableCollection<int> selectedSvids = new ObservableCollection<int>();
        [ObservableProperty]
        private int number;
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string? svid_list;
        [ObservableProperty]
        private bool state;
        [ObservableProperty]
        private List<int> sVIDs;
        [ObservableProperty]
        private string on_off;
        #endregion

        #region CONSTRUCTOR
        public CEIDModifyViewModel(IEventConfigManager configManager)
        {
            this._configManager = configManager;
        }
        #endregion

        #region COMMAND
        [RelayCommand]
        private void Save()
        {
            ChangeData();
            _configManager.UpdateCEIDSectionPartial(this._item);

            CloseRequested?.Invoke();
        }

        #endregion

        #region METHOD
        public void LoadItem(CEIDInfo item)
        {
            this._item = item;
            ChangeProperty();
        }

        private void ChangeProperty()
        {
            this.Number = this._item.Number;
            this.Name = this._item.Name;
            this.State = this._item.State;
            this.Svid_list = this._item.SVIDsDisplay;
            this.SVIDs = this._item.SVIDs;

            this.SelectedSvids.Clear();

            foreach (var svid in this.SVIDs)
            {
                this.SelectedSvids.Add(svid);
            }

            if (this.State)
                this.On_off = "ON";
            else
                this.On_off = "OFF";
        }

        private void ChangeData()
        {
            this._item.Number = this.Number;
            this._item.Name = this.Name;
            this._item.State = this.State;

            this._item.SVIDsDisplay = this.Svid_list;

            SVIDs.Clear();
            foreach (var svid in this.SelectedSvids)
            {
                this.SVIDs.Add(svid);
            }

            this._item.SVIDs = this.SVIDs;

            if (this.State)
                this.On_off = "ON";
            else
                this.On_off = "OFF";
        }
        #endregion
    }
}
