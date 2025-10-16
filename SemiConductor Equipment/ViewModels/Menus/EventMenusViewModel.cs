using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.ViewModels.Windows;
using SemiConductor_Equipment.Views.Pages;
using SemiConductor_Equipment.Views.Windows;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.ViewModels.Menus
{
    public partial class EventMenusViewModel : ObservableObject
    {
        #region FIELDS
        private IEventConfigManager _configManager;
        public Action modify_action;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private ObservableCollection<CEIDInfo> _cEID = new();
        [ObservableProperty]
        private ObservableCollection<RPTIDInfo> _rPTID = new();
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// CEID, RPTID 설정 클래스
        /// </summary>
        /// <param name="configManager"></param>
        public EventMenusViewModel(IEventConfigManager configManager) 
        {
            _configManager = configManager;
            _configManager.ConfigRead += OnConfigRead;
            _configManager.InitCEIDConfig();
            _configManager.InitRPTIDConfig();
        }
        #endregion

        #region COMMAND
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

        [RelayCommand]
        private void Add_RPTID()
        {
            var vm = App.Services.GetRequiredService<RPTIDAddViewModel>();
            vm.Clear();
            var window = new RPTIDAddWindow(vm);
            window.SetItem();
            window.ShowDialog();
        }


        #endregion

        #region METHOD
        /// <summary>
        /// CEID, RPTID 파일 Read 메서드
        /// </summary>
        private void OnConfigRead()
        {
            this.CEID = new ObservableCollection<CEIDInfo>(_configManager.CEID.Values);
            this.RPTID = new ObservableCollection<RPTIDInfo>(_configManager.RPTID.Values);
        }
        #endregion
    }
}
