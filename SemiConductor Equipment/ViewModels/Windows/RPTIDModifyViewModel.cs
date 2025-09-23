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
    public partial class RPTIDModifyViewModel : ObservableObject
    {
        #region FIELDS
        private RPTIDInfo _item;
        private readonly IEventConfigManager _configManager;
        public event Action CloseRequested;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private ObservableCollection<int> allVID = new ObservableCollection<int>{ 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116,
                                                                                    1001, 1002, 1003, 1005, 1007, 1008, 1009, 1010 };
        [ObservableProperty]
        private ObservableCollection<int> selectedSvids = new ObservableCollection<int>();
        [ObservableProperty]
        private int number;
        [ObservableProperty]
        private string? svid_list;
        [ObservableProperty]
        private List<int> vIDs;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// RPTID 수정 클래스
        /// </summary>
        /// <param name="configManager"></param>
        public RPTIDModifyViewModel(IEventConfigManager configManager)
        {
            this._configManager = configManager;
        }
        #endregion

        #region COMMAND
        /// <summary>
        /// 저장 커맨드
        /// </summary>
        [RelayCommand]
        private void Save()
        {
            ChangeData();
            _configManager.UpdateRPTIDSectionPartial(this._item);

            CloseRequested?.Invoke();
        }

        /// <summary>
        /// 삭제 커맨드
        /// </summary>
        [RelayCommand]
        private void Remove()
        {
            _configManager.RemoveRPTIDSectionPartial(this._item);

            CloseRequested?.Invoke();
        }

        #endregion

        #region METHOD
        /// <summary>
        /// 첫 화면 띄울 때 변수 저장 메서드
        /// </summary>
        /// <param name="item"></param>
        public void LoadItem(RPTIDInfo item)
        {
            this._item = item;
            ChangeProperty();
        }

        /// <summary>
        /// 초기값 설정 메서드
        /// </summary>
        private void ChangeProperty()
        {
            this.Number = this._item.Number;
            this.Svid_list = this._item.VIDsDisplay;
            this.VIDs = this._item.VIDs;

            this.SelectedSvids.Clear();

            foreach (var svid in this.VIDs)
            {
                this.SelectedSvids.Add(svid);
            }
        }

        /// <summary>
        /// 수정된 값 저장 메서드
        /// </summary>
        private void ChangeData()
        {
            this._item.Number = this.Number;

            this._item.VIDsDisplay = this.Svid_list;

            VIDs.Clear();
            foreach (var svid in this.SelectedSvids)
            {
                this.VIDs.Add(svid);
            }

            this._item.VIDs = this.VIDs;
        }
        #endregion
    }
}