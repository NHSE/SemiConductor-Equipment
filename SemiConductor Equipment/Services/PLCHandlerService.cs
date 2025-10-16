using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using static SemiConductor_Equipment.Enums.RegistersEnum; // NModbus4 네임스페이스

namespace SemiConductor_Equipment.Services
{
    public class PLCHandlerService : IPLCManager
    {
        #region FIELDS
        private TcpClient _client;
        private ModbusIpMaster _master;

        private readonly ILogManager _logManager;
        private readonly IMessageBox _messageBoxManager;

        public event EventHandler<ChamberRPMValue> ChangeRPMData;
        public event Action<bool> Server_Connect;

        public bool bNotConnect { get; set; }
        public bool _State { get; set; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public PLCHandlerService(ILogManager logManager, IMessageBox messageBox)
        {
            this._logManager = logManager;
            this._messageBoxManager = messageBox;

            bNotConnect = true;
            _State = false;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public async Task Initalize()
        {
            try
            {
                _client = new TcpClient("127.0.0.1", 502);
                _master = ModbusIpMaster.CreateIp(_client);

                if (_client.Connected)
                {

                    for (int time = 0; time < 30; time++)
                    {
                        // Master에 연결 신호 보내기
                        await _master.WriteSingleCoilAsync((int)Registers.Registers_Master_Connect, true);

                        // Slave 상태 읽기
                        bool[] connect = await _master.ReadCoilsAsync(0, 100);

                        if (connect[(int)Registers.Registers_Slave_Connect])
                        {
                            bNotConnect = false;
                            break;
                        }

                        // 1초 대기
                        await Task.Delay(1000);
                    }

                    Server_Connect?.Invoke(true);
                }
                else
                {
                    this._messageBoxManager.Show("예외 발생", $"TCP/IP 통신이 연결되지 않았습니다.\n가상 PLC를 확인해주세요.");
                }
            }
            catch (Exception ex)
            {
                await Server_End(false);
            }
        }

        public async Task Server_End(bool bClick)
        {
            try
            {
                if (bClick)
                {
                    await _master.WriteSingleCoilAsync((int)Registers.Registers_Master_Connect, false);
                    bNotConnect = true;
                    Server_Connect?.Invoke(false);
                    _client.Close();
                    _master.Dispose();
                }
                else
                {
                    bNotConnect = true;
                    Server_Connect?.Invoke(false);
                    _client.Close();
                    _master.Dispose();
                }
            }
            catch (Exception ex)
            {
                bNotConnect = true;
                Server_Connect?.Invoke(false);
                _client.Close();
                _master.Dispose();
                this._messageBoxManager.Show("예외 발생", $"서버 Accept 오류: {ex.Message}");
            }
        }

        private ushort Get_CleanRegisters(string chambername)
        {
            ushort ret = 0;
            switch (chambername)
            {
                case "Chamber1":
                    ret = (ushort)Registers.Registers_CleanChamber1;
                    break;

                case "Chamber2":
                    ret = (ushort)Registers.Registers_CleanChamber2;
                    break;

                case "Chamber3":
                    ret = (ushort)Registers.Registers_CleanChamber3;
                    break;

                case "Chamber4":
                    ret = (ushort)Registers.Registers_CleanChamber4;
                    break;

                case "Chamber5":
                    ret = (ushort)Registers.Registers_CleanChamber5;
                    break;

                case "Chamber6":
                    ret = (ushort)Registers.Registers_CleanChamber6;
                    break;
            }

            return ret;
        }

        private ushort Get_DryRegisters(string chambername)
        {
            ushort ret = 0;
            switch (chambername)
            {
                case "Chamber1":
                    ret = (ushort)Registers.Registers_DryChamber1;
                    break;

                case "Chamber2":
                    ret = (ushort)Registers.Registers_DryChamber2;
                    break;

                case "Chamber3":
                    ret = (ushort)Registers.Registers_DryChamber3;
                    break;

                case "Chamber4":
                    ret = (ushort)Registers.Registers_DryChamber4;
                    break;

                case "Chamber5":
                    ret = (ushort)Registers.Registers_DryChamber5;
                    break;

                case "Chamber6":
                    ret = (ushort)Registers.Registers_DryChamber6;
                    break;
            }

            return ret;
        }

        public async Task<int> PLC_Start(string chambername, int targetRpm, int currentRpm, bool bClean)
        {
            try
            {
                ushort Chamber_Write_Register = bClean ? Get_CleanRegisters(chambername) : Get_DryRegisters(chambername);
                ushort Chamber_Write_Coil = (ushort)(Chamber_Write_Register);
                ushort Chamber_Read_Register = (ushort)(Chamber_Write_Register);

                await _master.WriteSingleRegisterAsync(1, Chamber_Write_Register, (ushort)targetRpm); // 목표 RPM 전송
                await _master.WriteSingleCoilAsync(Chamber_Write_Coil, true);

                // 목표 RPM 도달할 때까지 반복
                while (currentRpm < targetRpm)
                {
                    ushort[] values = await _master.ReadInputRegistersAsync(1, Chamber_Read_Register, 1); // 현재 RPM 읽기
                    currentRpm = (int)values[0];

                    if (bClean)
                    {
                        this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] Rotational Speed : {(int)currentRpm} rpm");
                    }
                    else
                    {
                        ChangeRPMData?.Invoke(this, new ChamberRPMValue(chambername, currentRpm));
                        this._logManager.WriteLog($"Dry_{chambername}", $"State", $"[{chambername}] Rotational Speed : {(int)currentRpm} rpm");
                    }

                    await Task.Delay(500);
                }

                return currentRpm;
            }
            catch (Exception ex)
            {                
                await Server_End(false);
                return -1;
            }
        }

        public async Task<bool> PLC_Stop(string chambername, bool bClean)
        {
            try
            {
                ushort Chamber_Write_Register = bClean ? Get_CleanRegisters(chambername) : Get_DryRegisters(chambername);
                ushort Chamber_Write_Coil = (ushort)(Chamber_Write_Register);
                ushort Chamber_Read_Register = (ushort)(Chamber_Write_Register);

                ushort targetRpm = 0;
                await _master.WriteSingleRegisterAsync(1, Chamber_Write_Register, targetRpm); // 목표 RPM 전송
                await _master.WriteSingleCoilAsync(Chamber_Write_Coil, true);

                ushort[] values = await _master.ReadInputRegistersAsync(1, Chamber_Read_Register, 1); // 현재 RPM 읽기
                ushort currentRpm = values[0];

                // 목표 RPM 도달할 때까지 반복
                while (currentRpm > targetRpm)
                {
                    values = await _master.ReadInputRegistersAsync(1, Chamber_Read_Register, 1); // 현재 RPM 읽기
                    currentRpm = values[0];

                    if (bClean)
                    {
                        this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] Rotational Speed : {(int)currentRpm} rpm");
                    }
                    else
                    {
                        ChangeRPMData?.Invoke(this, new ChamberRPMValue(chambername, currentRpm));
                        this._logManager.WriteLog($"Dry_{chambername}", $"State", $"[{chambername}] Rotational Speed : {(int)currentRpm} rpm");
                    }

                    await Task.Delay(500);
                }

                return true;
            }
            catch (Exception ex)
            {
                await Server_End(false);

                return false;
            }
        }
    }
    #endregion
}
