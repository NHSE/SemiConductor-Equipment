using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.Views.Pages;
using SemiConductor_Equipment.Views.Windows;

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
        /// <summary>
        /// IP 설정 클래스
        /// </summary>
        /// <param name="configManager"></param>
        public IpSettingViewModel(IConfigManager configManager) 
        {
            _configManager = configManager;
            _configManager.ConfigRead += OnConfigRead;
        }

        #endregion

        #region COMMAND
        /// <summary>
        /// IP 초기화 커맨드
        /// </summary>
        [RelayCommand]
        private void Init()
        {
            _configManager.InitConfig();
        }

        /// <summary>
        /// IP 저장 커맨드
        /// </summary>
        [RelayCommand]
        private void Save()
        {
            _configManager.UpdateConfigValue("IP", this.IP);
            _configManager.UpdateConfigValue("Port", this.Port.ToString());
            _configManager.UpdateConfigValue("Device ID", this.DeviceID.ToString());

            _configManager.InitConfig();
        }

        [RelayCommand]
        private void Back()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var mainPage = App.Services.GetRequiredService<MainPage>();
                mainWindow.MainFrame.Navigate(mainPage);
            }
        }
        #endregion

        #region METHOD

        /// <summary>
        /// IP 정보 Read 메서드
        /// </summary>
        private void OnConfigRead()
        {
            this.IP = _configManager.IP;
            this.Port = _configManager.Port;
            this.DeviceID = _configManager.DeviceID;
        }
        #endregion
    }
}
