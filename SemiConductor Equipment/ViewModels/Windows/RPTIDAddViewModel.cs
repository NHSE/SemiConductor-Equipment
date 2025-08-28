using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.ViewModels.Windows
{
    public partial class RPTIDAddViewModel : ObservableObject
    {
        #region FIELDS
        private RPTIDInfo? _item;
        private readonly IEventConfigManager _configManager;
        private readonly IMessageBox _messageBoxManager;
        public event Action CloseRequested;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private ObservableCollection<int> allVID = new ObservableCollection<int> { 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116,
                                                                                    1001, 1002, 1003, 1005, 1007, 1008, 1009, 1010 };
        [ObservableProperty]
        private ObservableCollection<int> selectedSvids = new ObservableCollection<int>();
        [ObservableProperty]
        private string number;
        [ObservableProperty]
        private string? svid_list;
        [ObservableProperty]
        private List<int> vIDs = new List<int>();
        #endregion

        #region CONSTRUCTOR
        public RPTIDAddViewModel(IEventConfigManager configManager, IMessageBox messageBoxManager)
        {
            this._configManager = configManager;
            this._messageBoxManager = messageBoxManager;
        }
        #endregion

        #region COMMAND
        [RelayCommand]
        private void Save()
        {
            if(ChangeData())
            {
                _configManager.CreatedRPTIDSectionPartial(this._item);

                CloseRequested?.Invoke();
            }
        }

        #endregion

        #region METHOD
        public void Clear()
        {
            this.Number = "";
            this.VIDs = new List<int>();
            this.Svid_list = "";
            this.SelectedSvids.Clear();
        }

        public void LoadItem()
        {
            this._item = new RPTIDInfo();
        }

        private bool ChangeData()
        {
            try
            {
                this._item.Number = Convert.ToInt32(this.Number);

                if (this.Svid_list == string.Empty || SelectedSvids.Count == 0)
                    throw new Exception();

                this._item.VIDsDisplay = this.Svid_list;

                foreach (var svid in this.SelectedSvids)
                {
                    this.VIDs.Add(svid);
                }

                this._item.VIDs = this.VIDs;
            }
            catch (Exception ex)
            {
                this._messageBoxManager.Show("저장 오류", "저장할 수 없습니다.\n유효하지 않은 항목이 있습니다.");
                return false;
            }
            return true;
        }
        #endregion
    }
}
