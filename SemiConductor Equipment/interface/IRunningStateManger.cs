using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Enums;

namespace SemiConductor_Equipment.interfaces
{
    public interface IRunningStateManger
    {
        #region PROPERTIES
        #endregion

        #region METHODS
        event EventHandler<EquipmentStatusEnum> DataChange;

        void Change_State(object? sender, EquipmentStatusEnum state);

        public EquipmentStatusEnum Get_State();
        #endregion
    }
}
