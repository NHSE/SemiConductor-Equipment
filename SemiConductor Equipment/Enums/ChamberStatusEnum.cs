using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Enums
{
    public enum ChamberStatusEnum
    {
        Idle,       // 대기 중
        Running,    // 동작 중
        Error,      // 에러
        Completed   // 작업 완료
    }
}
