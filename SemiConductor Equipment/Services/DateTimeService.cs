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

        /// <summary>
        /// 현재 시간을 리턴해주는 메서드
        /// </summary>
        /// <returns>현재시간</returns>
        public DateTime? GetCurrentTime()
        {
            return DateTime.Now;
        }
        #endregion
    }
}
