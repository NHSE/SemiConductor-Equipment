using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Messages
{
    public class DeviceCommandMessage
    {
        public string Target { get; }      // 예: "Chamber1", "RobotArm", "Loader"
        public string Command { get; }     // 예: "Run", "Stop", "Reset"
        public object? Parameter { get; }  // 필요시 추가 데이터

        public DeviceCommandMessage(string target, string command, object? parameter = null)
        {
            Target = target;
            Command = command;
            Parameter = parameter;
        }
    }
}
