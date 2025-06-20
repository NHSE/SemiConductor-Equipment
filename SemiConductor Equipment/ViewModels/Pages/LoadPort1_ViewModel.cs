using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Navigation;
using CommunityToolkit.Mvvm.Messaging;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Messages;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class LoadPort1_ViewModel : ObservableObject, ILoadPortViewModel
    {
        #region FIELDS
        private readonly WaferService _waferService;
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
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsSetupEnabled")
            {
                if(!this.IsSetupEnabled)
                    WeakReferenceMessenger.Default.Send(new ViewModelMessages { Content = "LoadPort1_in_wafer" });
                else
                    WeakReferenceMessenger.Default.Send(new ViewModelMessages { Content = "LoadPort1" });
            }
        }
        #endregion

        #region COMMAND
        [RelayCommand]
        private void Cancel()
        {
            this.Waferinfo.Clear();
            this.SelectedSlots?.Clear();
            this.IsSetupEnabled = true;    // Setup 활성
            this.IsCancelEnabled = false;
        }
        #endregion

        #region METHOD
        public bool Update_Carrier_info(Wafer newWaferData)
        {
            if(SelectedSlots.Count == 0)
            {
                return false;
            }

            foreach (int slot in SelectedSlots)
            {
                var existingWafer = this.Waferinfo.FirstOrDefault(w =>
                    w.LoadportId == this.LoadPortId && w.Wafer_Num == slot);

                if (existingWafer != null)
                {
                    // 필요한 값만 갱신
                    if (!string.IsNullOrEmpty(newWaferData.CarrierId))
                    {
                        existingWafer.CarrierId = newWaferData.CarrierId;
                        this.CarrierId = existingWafer.CarrierId;
                    }
                    if (!string.IsNullOrEmpty(newWaferData.PJId))
                        existingWafer.PJId = newWaferData.PJId;
                    if (!string.IsNullOrEmpty(newWaferData.CJId))
                        existingWafer.CJId = newWaferData.CJId;
                    if (!string.IsNullOrEmpty(newWaferData.SlotId))
                        existingWafer.SlotId = newWaferData.SlotId;
                    if (!string.IsNullOrEmpty(newWaferData.LotId))
                        existingWafer.LotId = newWaferData.LotId;
                }
                else
                {
                    // 새 Wafer 추가: 이 경우는 그대로 복사
                    this.Waferinfo.Add(new Wafer
                    {
                        LoadportId = this.LoadPortId,
                        Wafer_Num = slot,
                        CarrierId = newWaferData.CarrierId ?? "",
                        PJId = newWaferData.PJId ?? "",
                        CJId = newWaferData.CJId ?? "",
                        SlotId = newWaferData.SlotId ?? "",
                        LotId = newWaferData.LotId ?? ""
                    });
                }
            }

            // CarrierId는 Wafer 단위로 관리하므로, 여기서 비교할 필요 없으면 생략 가능
            return true;
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
                    SlotId = slot.ToString("D2"),
                    LotId = ""
                });
            }
        }

        public string GetCarrierId()
        {
            return this.CarrierId;
        }

        public List<Wafer> GetAllWaferInfo(string pjid)
        {
            // 필요하다면 LoadPortId로 필터링
            return this.Waferinfo
                .Where(w => w.PJId == pjid)
                .ToList();
        }

        public string GetPJId(byte loadportId)
        {
            string pjid = this.Waferinfo[loadportId].PJId;
            return pjid;
        }
        #endregion
    }
}
