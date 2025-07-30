using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
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
        public EventMenusViewModel(IEventConfigManager configManager) 
        {
            _configManager = configManager;
            _configManager.ConfigRead += OnConfigRead;
            _configManager.InitCEIDConfig();
            _configManager.InitRPTIDConfig();
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        private void OnConfigRead()
        {
            this.CEID = new ObservableCollection<CEIDInfo>(_configManager.CEID.Values);
            this.RPTID = new ObservableCollection<RPTIDInfo>(_configManager.RPTID.Values);
        }
        #endregion
    }
}
