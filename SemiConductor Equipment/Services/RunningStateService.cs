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

        public void Change_State(object? sender, EquipmentStatusEnum state)
        {
            if(state == EquipmentStatusEnum.Running)
            {
                this._state = EquipmentStatusEnum.Running;
                DataChange?.Invoke(sender, EquipmentStatusEnum.Running);
            }
            else if(state == EquipmentStatusEnum.Completed)
            {
                this._state = EquipmentStatusEnum.Completed;
                DataChange?.Invoke(sender, EquipmentStatusEnum.Completed);
            }
            else if (state == EquipmentStatusEnum.Error)
            {
                this._state = EquipmentStatusEnum.Error;
                DataChange?.Invoke(sender, EquipmentStatusEnum.Error);
            }
            else if (state == EquipmentStatusEnum.Wait)
            {
                this._state = EquipmentStatusEnum.Wait;
                DataChange?.Invoke(sender, EquipmentStatusEnum.Wait);
            }
            else
            {
                this._state = EquipmentStatusEnum.Ready;
                DataChange?.Invoke(sender, EquipmentStatusEnum.Ready);
            }
        }

        public EquipmentStatusEnum Get_State()
        {
            return this._state;
        }
    }
}
