using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Models
{
    public partial class OHTCarrierInfo : ObservableObject
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private List<int> _carrierInfo = new();

        [ObservableProperty]
        private int _loadPort;
        #endregion

        #region CONSTRUCTOR
        public OHTCarrierInfo(List<int> newValue1, int newValue2)
        {
            CarrierInfo = newValue1;
            LoadPort = newValue2;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        #endregion
    }

}