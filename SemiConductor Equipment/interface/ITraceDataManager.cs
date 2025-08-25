using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;

namespace SemiConductor_Equipment.interfaces
{
    public interface ITraceDataManager
    {
        #region PROPERTIES
        #endregion

        #region METHODS
        void SetSecsGem(ISecsGem secsGem);
        void SetConnect(ISecsConnection connection);
        bool SetTraceData(string TRID, string DSPER, uint TOTSMP, uint REPGSZ, List<uint> VID);
        #endregion
    }
}
