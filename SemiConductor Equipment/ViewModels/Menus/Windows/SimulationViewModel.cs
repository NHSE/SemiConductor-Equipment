using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.ViewModels.Menus.Windows
{
    public partial class SimulationViewModel : ObservableObject
    {
        #region FIELDS
        private readonly ISimulationManager _simulationManager;
        private readonly IRunningStateManger _runningStateManger;
        private readonly IPLCManager _PLCManager;
        private readonly IOHTManager _OHTManager;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        public bool _state = false;

        [ObservableProperty]
        public string _oht = "DisConnected";

        [ObservableProperty]
        public string _plc = "DisConnected";

        [ObservableProperty]
        public Brush _ohtColor = Brushes.Red;

        [ObservableProperty]
        public Brush _plcColor = Brushes.Red;

        [ObservableProperty]
        public bool _isProcessing = true;
        #endregion

        #region CONSTRUCTOR
        public SimulationViewModel(ISimulationManager simulationManager, IPLCManager pLCManager, IOHTManager OHTManager, IRunningStateManger runningStateManger)
        {
            this._simulationManager = simulationManager;
            this._PLCManager = pLCManager;
            this._OHTManager = OHTManager;
            this._runningStateManger = runningStateManger;

            this._simulationManager.ConfigRead += OnConfigRead;
            this._OHTManager.Server_Connect += OHT_Server_Connect;
            this._PLCManager.Server_Connect += PLC_Server_Connect;
            this._runningStateManger.DataChange += OnDataChange;

            this._simulationManager.InitConfig();
        }
        #endregion

        #region COMMAND
        [RelayCommand]
        public void OHT_Connect()
        {
            this.Oht = "Connecting ... ";
            this.OhtColor = Brushes.Yellow;
            this._OHTManager.Initalize();
        }

        [RelayCommand]
        public void OHT_DisConnect()
        {
            this._OHTManager.Stop();
        }

        [RelayCommand]
        public void PLC_Connect()
        {
            this.Plc = "Connecting ... ";
            this.PlcColor = Brushes.Yellow;
            Task.Run(() => this._PLCManager.Initalize());
        }

        [RelayCommand]
        public void PLC_DisConnect()
        {
            this.Plc = "DisConnected";
            this.PlcColor = Brushes.Red;
            Task.Run(() => this._PLCManager.Server_End(true));
        }
        #endregion

        #region METHOD
        partial void OnStateChanged(bool oldValue, bool newValue)
        {
            if (oldValue != newValue)
            {
                this._simulationManager.UpdateConfigValue(newValue);
            }
        }

        private void OnConfigRead()
        {
            this.State = this._simulationManager.State;
        }

        private void OHT_Server_Connect()
        {
            if(this._OHTManager._State)
            {
                this.Oht = "Connected";
                this.OhtColor = Brushes.LightGreen;
            }
            else
            {
                this.Oht = "DisConnected";
                this.OhtColor = Brushes.Red;
            }
        }

        private void PLC_Server_Connect(bool state)
        {
            if (state)
            {
                this.Plc = "Connected";
                this.PlcColor = Brushes.LightGreen;
            }
            else
            {
                this.Plc = "DisConnected";
                this.PlcColor = Brushes.Red;
            }

            this._PLCManager._State = state;
        }

        private void OnDataChange(object? sender, EquipmentStatusEnum e)
        {
            if(e == EquipmentStatusEnum.Running)
            {
                this.IsProcessing = false;
            }
            else
            {
                this.IsProcessing = true;
            }
        }
        #endregion
    }
}
