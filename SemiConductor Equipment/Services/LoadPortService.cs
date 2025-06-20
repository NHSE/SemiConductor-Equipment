using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.Services
{
    public class LoadPortService
    {
        private readonly object _lock = new();

        // 포트 ID별로 슬롯 25개 (0~24): 각 슬롯에 Wafer 저장
        private readonly Dictionary<string, Wafer?[]> _loadPorts = new()
        {
            ["LoadPort1"] = new Wafer?[25],
            ["LoadPort2"] = new Wafer?[25]
        };

        public Wafer? PickWaferFromPort(string loadPortId)
        {
            lock (_lock)
            {
                if (!_loadPorts.ContainsKey(loadPortId)) return null;

                var slots = _loadPorts[loadPortId];
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i] != null)
                    {
                        var wafer = slots[i];
                        slots[i] = null;
                        Console.WriteLine($"[LoadPort] Picked {wafer.SlotId} from {loadPortId} Slot {i + 1}");
                        return wafer;
                    }
                }

                return null;
            }
        }

        public bool PlaceWaferInPort(string loadPortId, Wafer wafer)
        {
            lock (_lock)
            {
                if (!_loadPorts.ContainsKey(loadPortId)) return false;

                var slots = _loadPorts[loadPortId];
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i] == null)
                    {
                        slots[i] = wafer;
                        Console.WriteLine($"[LoadPort] Placed {wafer.SlotId} into {loadPortId} Slot {i + 1}");
                        return true;
                    }
                }

                Console.WriteLine($"[LoadPort] No empty slot available in {loadPortId}.");
                return false;
            }
        }

        public void SetInitialWafers(string loadPortId, List<Wafer> wafers)
        {
            lock (_lock) // 멀티스레드 환경에서 데이터 충돌 방지
            {
                // 해당 LoadPort가 존재하지 않으면 함수 종료
                if (!_loadPorts.ContainsKey(loadPortId))
                    return;

                // LoadPort의 슬롯 배열을 가져옴
                var slots = _loadPorts[loadPortId]; // slots는 Wafer[] 또는 List<Wafer> 여야 함

                // slots와 wafers 중 더 작은 개수만큼만 복사
                for (int i = 0; i < Math.Min(slots.Length, wafers.Count); i++)
                {
                    slots[i] = wafers[i]; // 각 슬롯에 Wafer 정보 할당
                }
            }
        }

        public List<Wafer> GetAllWafers(string loadPortId)
        {
            lock (_lock)
            {
                if (!_loadPorts.ContainsKey(loadPortId)) return new List<Wafer>();

                return _loadPorts[loadPortId]
                    .Where(w => w != null)
                    .Select(w => w!)
                    .ToList();
            }
        }
    }
}
