using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Dtos;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.ViewModels.Menus
{
    public partial class EquipMenusViewModel : ObservableObject
    {
        #region FIELDS
        private readonly IEquipmentConfigManager _configManager;
        private readonly IMessageBox _messageBoxManager;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private int _clean_rpm;
        [ObservableProperty]
        private int _flowRate;
        [ObservableProperty]
        private int _spraytime;
        [ObservableProperty]
        private int _preClean_FlowRate;
        [ObservableProperty]
        private int _preClean_Spraytime;
        [ObservableProperty]
        private int _chambertime;
        [ObservableProperty]
        private int _max_temperature;
        [ObservableProperty]
        private int _min_temperature;
        [ObservableProperty]
        private int _dry_Rpm;
        #endregion

        #region CONSTRUCTOR
        public EquipMenusViewModel(IEquipmentConfigManager configManager, IMessageBox messageBoxManager)
        {
            _configManager = configManager;
            _messageBoxManager = messageBoxManager;
            _configManager.ConfigRead += OnConfigRead;
            _configManager.InitConfig();
        }
        #endregion

        #region COMMAND
        [RelayCommand]
        private void Init()
        {
            _configManager.InitConfig();
        }

        [RelayCommand]
        private void Save()
        {
            if(this.Clean_rpm < 10 || this.Dry_Rpm < 10)
            {
                this._messageBoxManager.Show("입력 오류", "RPM 입력은 10 이상 3000이하만 가능합니다.");
                return;
            }

            _configManager.UpdateConfigValue("Clean RPM", this.Clean_rpm);
            _configManager.UpdateConfigValue("Chemical Flow Rate", this.FlowRate);
            _configManager.UpdateConfigValue("Chemical Spray Time", this.Spraytime);
            _configManager.UpdateConfigValue("Pre-Clean Flow Rate", this.PreClean_FlowRate);
            _configManager.UpdateConfigValue("Pre-Clean Spray Time", this.PreClean_Spraytime);

            _configManager.UpdateConfigValue("Max Temperature", this.Max_temperature);
            _configManager.UpdateConfigValue("Min Temperature", this.Min_temperature);
            _configManager.UpdateConfigValue("Dry RPM", this.Dry_Rpm);
            _configManager.UpdateConfigValue("Chamber Time", this.Chambertime);

            _configManager.InitConfig();
        }
        #endregion

        #region METHOD
        private void OnConfigRead()
        {
            this.Clean_rpm = _configManager.Clean_RPM;
            this.FlowRate = _configManager.Flow_Rate;
            this.Spraytime = _configManager.Spray_Time;
            this.PreClean_FlowRate = _configManager.PreClean_Flow_Rate;
            this.PreClean_Spraytime = _configManager.PreClean_Spray_Time;

            this.Max_temperature = _configManager.Max_Temp;
            this.Min_temperature = _configManager.Min_Temp;
            this.Dry_Rpm = _configManager.Dry_RPM;
            this.Chambertime = _configManager.Chamber_Time;
        }
        #endregion
    }
}
