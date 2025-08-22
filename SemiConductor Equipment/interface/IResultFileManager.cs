using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.interfaces
{
    public interface IResultFileManager
    {
        #region PROPERTIES
        string ProcessTime { get; set; }
        #endregion

        #region METHODS
        void ClearData();

        void InsertData(string ChamberType, LoadPortWaferKey key, ResultData value);

        void SaveFile(bool isClean);

        void SetProcessTime(string time);
        #endregion
    }
}
