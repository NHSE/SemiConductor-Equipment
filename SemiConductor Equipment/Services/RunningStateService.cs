using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Enums;

namespace SemiConductor_Equipment.Services
{
    public class RunningStateService
    {
        private EquipmentStatusEnum _state;
        public event EventHandler<EquipmentStatusEnum> DataChange;

        public void Change_State(EquipmentStatusEnum state)
        {
            if(state == EquipmentStatusEnum.Running)
            {
                this._state = EquipmentStatusEnum.Running;
                DataChange?.Invoke(this, EquipmentStatusEnum.Running);
            }
            else if(state == EquipmentStatusEnum.Completed)
            {
                this._state = EquipmentStatusEnum.Completed;
                DataChange?.Invoke(this, EquipmentStatusEnum.Completed);
            }
            else if (state == EquipmentStatusEnum.Error)
            {
                this._state = EquipmentStatusEnum.Error;
                DataChange?.Invoke(this, EquipmentStatusEnum.Error);
            }
            else if (state == EquipmentStatusEnum.Wait)
            {
                this._state = EquipmentStatusEnum.Wait;
                DataChange?.Invoke(this, EquipmentStatusEnum.Wait);
            }
        }

        public EquipmentStatusEnum Get_State()
        {
            return this._state;
        }
    }
}
