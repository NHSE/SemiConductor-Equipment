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
        private int _max_temperature;
        [ObservableProperty]
        private int _min_temperature;
        [ObservableProperty]
        private int _allowable;
        [ObservableProperty]
        private int _chambertime;
        #endregion

        #region CONSTRUCTOR
        public EquipMenusViewModel(IEquipmentConfigManager configManager)
        {
            _configManager = configManager;
            _configManager.ConfigRead += OnConfigRead;
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
            this.Max_temperature = _configManager.Max_Temp;
            this.Min_temperature = _configManager.Min_Temp;
            this.Allowable = _configManager.Allow;
            this.Chambertime = _configManager.Chamber_Time;
        }
        #endregion
    }
}
