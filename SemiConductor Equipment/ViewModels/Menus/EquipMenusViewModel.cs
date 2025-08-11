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
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private int _rpm;
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
        private int _allowable;
        #endregion

        #region CONSTRUCTOR
        public EquipMenusViewModel(IEquipmentConfigManager configManager)
        {
            _configManager = configManager;
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
            _configManager.UpdateConfigValue("RPM", this.Rpm);
            _configManager.UpdateConfigValue("Chemical Flow Rate", this.FlowRate);
            _configManager.UpdateConfigValue("Chemical Spray Time", this.Spraytime);
            _configManager.UpdateConfigValue("Pre-Clean Flow Rate", this.PreClean_FlowRate);
            _configManager.UpdateConfigValue("Pre-Clean Spray Time", this.PreClean_Spraytime);

            _configManager.UpdateConfigValue("Max Temperature", this.Max_temperature);
            _configManager.UpdateConfigValue("Min Temperature", this.Min_temperature);
            _configManager.UpdateConfigValue("Allowable", this.Allowable);
            _configManager.UpdateConfigValue("Chamber Time", this.Chambertime);

            _configManager.InitConfig();
        }
        #endregion

        #region METHOD
        private void OnConfigRead()
        {
            this.Rpm = _configManager.RPM;
            this.FlowRate = _configManager.Flow_Rate;
            this.Spraytime = _configManager.Spray_Time;
            this.PreClean_FlowRate = _configManager.PreClean_Flow_Rate;
            this.PreClean_Spraytime = _configManager.PreClean_Spray_Time;

            this.Max_temperature = _configManager.Max_Temp;
            this.Min_temperature = _configManager.Min_Temp;
            this.Allowable = _configManager.Allow;
            this.Chambertime = _configManager.Chamber_Time;
        }
        #endregion
    }
}
