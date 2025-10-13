using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Models
{
    public partial class AlarmMessage : ObservableObject
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private int _alarmNum;

        [ObservableProperty]
        private string? _alarmMsg;
        [ObservableProperty]
        private DateTime? _alarmTime;
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        #endregion
    }
}
