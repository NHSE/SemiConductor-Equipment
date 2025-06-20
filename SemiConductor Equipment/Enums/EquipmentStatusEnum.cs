using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Enums
{
    public enum EquipmentStatusEnum
    {
        Ready,       // 대기 중
        Running,    // 동작 중
        Completed,   // 작업 완료
        Error
    }
}
