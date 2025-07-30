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
        private ObservableCollection<RPTIDInfo> RPTID_LIST;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private ObservableCollection<int> allRPTID;
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
        private List<int> rPTIDs;
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
            this.Svid_list = this._item.RPTIDsDisplay;
            this.RPTIDs = this._item.RPTIDs;

            this.SelectedSvids.Clear();

            foreach (var svid in this.RPTIDs)
            {
                this.SelectedSvids.Add(svid);
            }

            if (this.State)
                this.On_off = "ON";
            else
                this.On_off = "OFF";

            RPTID_LIST = new ObservableCollection<RPTIDInfo>(_configManager.RPTID.Values);

            this.AllRPTID = new ObservableCollection<int>(RPTID_LIST.SelectMany(r => r.Number_list));
        }

        private void ChangeData()
        {
            this._item.Number = this.Number;
            this._item.Name = this.Name;
            this._item.State = this.State;

            this._item.RPTIDsDisplay = this.Svid_list;

            RPTIDs.Clear();
            foreach (var svid in this.SelectedSvids)
            {
                this.RPTIDs.Add(svid);
            }

            this._item.RPTIDs = this.RPTIDs;

            if (this.State)
                this.On_off = "ON";
            else
                this.On_off = "OFF";
        }
        #endregion
    }
}
