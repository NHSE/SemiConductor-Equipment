using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    class DateTimeService : IDateTime
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public DateTime? GetCurrentTime()
        {
            return DateTime.Now;
        }
        #endregion
    }
}
