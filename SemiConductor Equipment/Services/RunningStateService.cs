using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    public class RunningStateService : IRunningStateManger
    {
        #region FIELDS
        private EquipmentStatusEnum _state;
        public event EventHandler<EquipmentStatusEnum> DataChange;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD

        /// <summary>
        /// 장비 상태 변경하는 메서드 (이벤트로 상태값 변경)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
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

        /// <summary>
        /// 현재 장비의 상태를 확인하는 메서드
        /// </summary>
        /// <returns>장비 상태</returns>
        public EquipmentStatusEnum Get_State()
        {
            return this._state;
        }
        #endregion
    }
}
