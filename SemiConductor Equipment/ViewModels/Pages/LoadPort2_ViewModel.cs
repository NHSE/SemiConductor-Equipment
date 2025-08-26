using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Messages;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class LoadPort2_ViewModel : ObservableObject, ILoadPortViewModel
    {
        #region FIELDS
        public event EventHandler<Wafer> RemoveRequested;
        public event EventHandler<Wafer> AddRequested;
        private readonly IRobotArmManager _robotArmManager;
        private readonly RunningStateService _runningStateService;
        private readonly IVIDManager _vIDManager;
        private readonly IEventMessageManager _eventMessageManager;
        private readonly IWaferProcessCoordinator _processManager;
        public byte LoadPortId => 2;
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

        [ObservableProperty]
        private string? _lPState = "Ready";
        #endregion

        #region CONSTRUCTOR
        public LoadPort2_ViewModel(IRobotArmManager robotArmManager, RunningStateService runningStateService, IVIDManager VIDManager, 
            IEventMessageManager eventMessageManager, IWaferProcessCoordinator processManager)
        {
            this._robotArmManager = robotArmManager;
            this._runningStateService = runningStateService;
            this._vIDManager = VIDManager;
            this._eventMessageManager = eventMessageManager;
            this._processManager = processManager;

            this._robotArmManager.CommandStarted += OnWaferOut;
            this._robotArmManager.CommandCompleted += OnWaferIn;
            this._runningStateService.DataChange += OnEquipment_State_Change;
            this._processManager.Process += ProcessChange;

            PropertyChanged += OnPropertyChanged;
        }
        #endregion

        #region COMMAND
        [RelayCommand]
        private void Cancel()
        {
            EquipmentStatusEnum state = EquipmentStatusEnum.Ready;
            this._runningStateService.Change_State("LoadPort2", state);
            _waferinfo.Clear();
        }
        #endregion

        #region METHOD

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSetupEnabled")
            {
                if (!this.IsSetupEnabled && _waferinfo.Count > 0)
                    WeakReferenceMessenger.Default.Send(new ViewModelMessages { Content = "LoadPort2_in_wafer" });
                else if (this.IsSetupEnabled && _waferinfo.Count == 0)
                    WeakReferenceMessenger.Default.Send(new ViewModelMessages { Content = "LoadPort2" });
            }
        }

        public bool Update_Carrier_info(Wafer newWaferData)
        {
            if (SelectedSlots == null || SelectedSlots.Count == 0)
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

                if(existingWafer.LotId != string.Empty)
                {
                    this._vIDManager.SetDVID(1004, existingWafer.LotId, existingWafer.Wafer_Num);
                }
                if(existingWafer.SlotId != string.Empty)
                {
                    this._vIDManager.SetDVID(1005, existingWafer.SlotId, existingWafer.Wafer_Num);
                }
            }
            return true;
        }

        public bool Check_Running(string cjid)
        {
            if (SelectedSlots == null || SelectedSlots.Count == 0)
            {
                return false;
            }

            foreach (int slot in SelectedSlots)
            {
                var existingWafer = this.Waferinfo.FirstOrDefault(w =>
                    w.LoadportId == this.LoadPortId && w.Wafer_Num == slot);
                if (!string.IsNullOrEmpty(existingWafer.CJId))
                {
                    return false;
                }
            }
            return true;
        }

        partial void OnSelectedSlotsChanged(List<int> oldValue, List<int> newValue)
        {
            if (newValue == null) return;

            this.Waferinfo.Clear();
            Random random = new Random();
            string carrierId = this.CarrierId ?? "UNKNOWN";

            foreach (int slot in newValue.OrderBy(x => x))
            {
                double temperature = random.Next(20, 30);
                this.Waferinfo.Add(new Wafer
                {
                    LoadportId = this.LoadPortId,
                    Wafer_Num = slot,
                    CarrierId = carrierId,
                    PJId = "",
                    CJId = "",
                    SlotId = slot.ToString("D2"),
                    LotId = "",
                    CurrentLocation = $"LoadPort{this.LoadPortId}",
                    RequiredTemperature = temperature,
                    RunningTime = 0.0,
                });

                this._vIDManager?.SetDVID(1001, (int)temperature, slot);
            }
            this._vIDManager?.SetDVID(1002, newValue.Count(), LoadPortId);
            this._vIDManager?.SetSVID(103, "CLOSE");
            LoadPortCompleted();
        }

        private void LoadPortCompleted()
        {
            CEIDInfo info = this._eventMessageManager.GetCEID(100);
            info.Loadport_Number = this.LoadPortId;
            info.Wafer_List = this.Waferinfo.Select(w => w.Wafer_Num).ToList();
            this._eventMessageManager.EnqueueEventData(info);
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
            var wafer = Waferinfo.FirstOrDefault(w => w.LoadportId == loadportId);
            return wafer?.PJId ?? "";
        }

        private void OnEquipment_State_Change(object? sender, EquipmentStatusEnum state)
        {
            if ((sender as string) == "LoadPort2")
            {
                if (state == EquipmentStatusEnum.Running)
                {
                    this.LPState = "Running";
                    this.IsCancelEnabled = false;
                }
                else if (state == EquipmentStatusEnum.Completed)
                {
                    this.LPState = "Completed";
                    this.IsCancelEnabled = true;
                }
                else if (state == EquipmentStatusEnum.Wait)
                {
                    this.LPState = "Wait";
                    this.IsCancelEnabled = false;
                }
                else
                {
                    this.Waferinfo.Clear();
                    this.SelectedSlots?.Clear();
                    this.IsSetupEnabled = true;
                    this.IsCancelEnabled = false;
                    this.LPState = "Ready";
                    if (!string.IsNullOrEmpty(this.CarrierId))
                    {
                        this.CarrierId = "";
                    }
                }
            }
        }

        private void OnWaferIn(object? sender, Wafer e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e.LoadportId == this.LoadPortId)
                {
                    AddRequested?.Invoke(this, e);
                }
            });

        }

        private void OnWaferOut(object? sender, Wafer e)
        {
            if (e.LoadportId == this.LoadPortId)
            {
                RemoveRequested?.Invoke(this, e);
            }
        }

        private void ProcessChange(object? sender, string e)
        {
            if (e == "Start")
            {
                this.IsSetupEnabled = false;
                this.IsCancelEnabled = false;
            }
            else if(e == "END" && this.LPState == "Completed")
            {
                this.IsSetupEnabled = false;
                this.IsCancelEnabled = true;
            }
            else
            {
                this.IsSetupEnabled = true;
                this.IsCancelEnabled = false;
            }
        }
        #endregion
    }
}
