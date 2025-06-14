using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.ViewModels.Menus
{
    public partial class IpSettingViewModel : ObservableObject // temp
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private string? _iP = "127.0.0.1";
        [ObservableProperty]
        private ushort? _deviceID = 1;
        [ObservableProperty]
        private int? _port = 5000;
        #endregion

        #region CONSTRUCTOR
        public IpSettingViewModel() 
        {
            
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        #endregion
    }
}
