using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.interfaces
{
    public interface IWaferManager
    {
        #region PROPERTIES
        #endregion

        #region METHODS
        void Enqueue(Wafer wafer);

        Wafer? Dequeue();

        Queue<Wafer> GetQueue();

        void Clear();
        #endregion
    }
}
