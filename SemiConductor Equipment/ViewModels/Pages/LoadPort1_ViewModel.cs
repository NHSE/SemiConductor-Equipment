using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Navigation;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class LoadPort1_ViewModel : ObservableObject, ILoadPortViewModel
    {
        #region FIELDS
        private readonly WaferService _waferService;
        private Wafer _wafer;
        public byte LoadPortId => 1;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private string? _carrierId;

        [ObservableProperty]
        private ObservableCollection<Wafer> _waferinfo = new();

        [ObservableProperty]
        public List<int> _selectedSlots;

        [ObservableProperty]
        private bool _isSetupEnabled = true;

        [ObservableProperty]
        private bool _isCancelEnabled = false;
        #endregion

        #region CONSTRUCTOR
        public LoadPort1_ViewModel(WaferService waferService)
        {
            _waferService = waferService;
        }
        #endregion

        #region COMMAND
        [RelayCommand]
        private void Cancel()
        {
            this.Waferinfo.Clear();
            this.SelectedSlots?.Clear();
            IsSetupEnabled = true;    // Setup 활성
            IsCancelEnabled = false;
        }
        #endregion

        #region METHOD
        public bool Update_Carrier_info(string? carrierId)
        {
            this.CarrierId = carrierId;
            foreach (int slot in SelectedSlots)
            {
                var existingWafer = this.Waferinfo.FirstOrDefault(w => w.LoadportId == this.LoadPortId && w.Wafer_Num == slot);

                if (existingWafer != null)
                {
                    // 기존 객체가 있으면 값 덮어쓰기
                    existingWafer.CarrierId = carrierId;
                    existingWafer.PJId = "";
                    existingWafer.CJId = "";
                    existingWafer.SlotId = "";
                    existingWafer.LotId = "";
                }
                else
                {
                    // 없으면 새 객체 추가
                    this.Waferinfo.Add(new Wafer
                    {
                        LoadportId = this.LoadPortId,
                        Wafer_Num = slot,
                        CarrierId = carrierId,
                        PJId = "",
                        CJId = "",
                        SlotId = "",
                        LotId = ""
                    });
                }
            }
            return this.CarrierId == carrierId;
        }

        partial void OnSelectedSlotsChanged(List<int> oldValue, List<int> newValue)
        {
            if (newValue == null) return;

            this.Waferinfo.Clear();
            string carrierId = this.CarrierId ?? "UNKNOWN";

            foreach (int slot in newValue.OrderBy(x => x))
            {
                this.Waferinfo.Add(new Wafer
                {
                    LoadportId = this.LoadPortId,
                    Wafer_Num = slot,
                    CarrierId = carrierId,
                    PJId = "",
                    CJId = "",
                    SlotId = "",
                    LotId = ""
                });
            }
        }

        public void HandlePJCommand(string PJjobId)
        {
            this.Waferinfo.Clear();

            foreach (int slot in SelectedSlots.OrderBy(x => x))
            {
                this.Waferinfo.Add(new Wafer
                {
                    LoadportId = this.LoadPortId,
                    CarrierId = CarrierId ?? "UNKNOWN",
                    Wafer_Num = slot,
                    PJId = PJjobId,
                    CJId = "",
                    SlotId = PJjobId + "." + slot,
                    LotId = $"Lot_{CarrierId}_{slot}"
                });
            }
        }
        #endregion
    }
}
