using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;

namespace SemiConductor_Equipment.ViewModels.Menus
{
    public partial class IpSettingViewModel : ObservableObject // temp
    {
        #region FIELDS
        private readonly IConfigManager _configManager;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private string? _iP;
        [ObservableProperty]
        private ushort? _deviceID;
        [ObservableProperty]
        private int? _port;
        #endregion

        #region CONSTRUCTOR
        public IpSettingViewModel(IConfigManager configManager) 
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
            _configManager.UpdateConfigValue("IP", this.IP);
            _configManager.UpdateConfigValue("Port", this.Port.ToString());
            _configManager.UpdateConfigValue("Device ID", this.DeviceID.ToString());
        }
        #endregion

        #region METHOD

        private void OnConfigRead()
        {
            this.IP = _configManager.IP;
            this.Port = _configManager.Port;
            this.DeviceID = _configManager.DeviceID;
        }
        #endregion
    }
}
