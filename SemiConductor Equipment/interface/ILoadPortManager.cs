using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.interfaces
{
    public interface ILoadPortManager
    {
        #region PROPERTIES
        #endregion

        #region METHODS
        Wafer? PickWaferFromPort(string loadPortId);

        bool PlaceWaferInPort(string loadPortId, Wafer wafer);

        void SetInitialWafers(string loadPortId, List<Wafer> wafers);

        List<Wafer> GetAllWafers(string loadPortId);
        #endregion
    }
}
