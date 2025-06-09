using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    public class ChamberService : IChamberService
    {
        private readonly Dictionary<int, ChamberStatusEnum> _chamberStatus = new();
        private readonly ILogManager? _logManager;
        public event Action<string>? ErrorOccurred;
        public void RunChamber(int chamberNo)
        {
            if (GetStatus(chamberNo) == ChamberStatusEnum.Running)
            {
                _logManager?.WriteLog($"Chamber{chamberNo}", "Warning", "Already Running");
                ErrorOccurred?.Invoke("챔버 동작 중 오류가 발생했습니다!");
                return;
            }
            // 실제 챔버 동작 로직 (예시)
            SetStatus(chamberNo, (int)ChamberStatusEnum.Running);

            // 실제 장비 제어 코드가 들어갈 수 있습니다.
        }

        public void SetStatus(int chamberNo, int state)
        {
            var status = (ChamberStatusEnum)state;
            this._chamberStatus[chamberNo] = status;
        }

        public ChamberStatusEnum GetStatus(int chamberNo)
        {
            if (this._chamberStatus.TryGetValue(chamberNo, out var status))
                return status;
            return ChamberStatusEnum.Idle;
        }
    }
}
