using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.interfaces
{
    public interface IDBLogManager
    {
        #region PROPERTIES
        #endregion

        #region METHODS
        void WriteDbLog(string Alarm_Msg);
        #endregion

        #region EVENTS
        #endregion
    }
}
