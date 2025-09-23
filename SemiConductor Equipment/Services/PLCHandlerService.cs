using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.interfaces;
using static SemiConductor_Equipment.Enums.RegistersEnum; // NModbus4 네임스페이스

namespace SemiConductor_Equipment.Services
{
    public class PLCHandlerService : IPLCManager
    {
        #region FIELDS
        private TcpClient _client;
        private ModbusIpMaster _master;

        private readonly ILogManager _logManager;
        private readonly IMessageBox _messageBox;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public PLCHandlerService(ILogManager logManager, IMessageBox messageBox)
        {
            this._logManager = logManager;
            this._messageBox = messageBox;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public void Initalize()
        {
            try
            {
                _client = new TcpClient("127.0.0.1", 502);
                _master = ModbusIpMaster.CreateIp(_client);
            }
            catch (Exception ex)
            {
                //return;
            }
        }

        public byte Get_CleanRegisters(string chambername)
        {
            byte ret = 0;
            switch (chambername)
            {
                case "Chamber1":
                    ret = (byte)Registers.Registers_CleanChamber1;
                    break;

                case "Chamber2":
                    ret = (byte)Registers.Registers_CleanChamber2;
                    break;

                case "Chamber3":
                    ret = (byte)Registers.Registers_CleanChamber3;
                    break;

                case "Chamber4":
                    ret = (byte)Registers.Registers_CleanChamber4;
                    break;

                case "Chamber5":
                    ret = (byte)Registers.Registers_CleanChamber5;
                    break;

                case "Chamber6":
                    ret = (byte)Registers.Registers_CleanChamber6;
                    break;
            }

            return ret;
        }

        public byte Get_DryRegisters(string chambername)
        {
            byte ret = 0;
            switch (chambername)
            {
                case "Chamber1":
                    ret = (byte)Registers.Registers_DryChamber1;
                    break;

                case "Chamber2":
                    ret = (byte)Registers.Registers_DryChamber2;
                    break;

                case "Chamber3":
                    ret = (byte)Registers.Registers_DryChamber3;
                    break;

                case "Chamber4":
                    ret = (byte)Registers.Registers_DryChamber4;
                    break;

                case "Chamber5":
                    ret = (byte)Registers.Registers_DryChamber5;
                    break;

                case "Chamber6":
                    ret = (byte)Registers.Registers_DryChamber6;
                    break;
            }

            return ret;
        }

        public async Task<int> Start(string chambername, int targetRpm, int currentRpm, bool bClean)
        {
            try
            {
                byte Chamber_Write_Register = bClean ? Get_CleanRegisters(chambername) : Get_DryRegisters(chambername);
                byte Chamber_Read_Register = (byte)(Chamber_Write_Register + 1);

                bool flag = true;
                await _master.WriteSingleRegisterAsync(1, Chamber_Write_Register, (ushort)targetRpm); // 목표 RPM 전송
                await _master.WriteSingleCoilAsync(Chamber_Write_Register, true);

                // 목표 RPM 도달할 때까지 반복
                while (currentRpm < targetRpm)
                {
                    ushort[] values = await _master.ReadHoldingRegistersAsync(1, Chamber_Read_Register, 1); // 현재 RPM 읽기
                    currentRpm = (int)values[0];

                    if (bClean)
                    {
                        this._logManager.WriteLog($"Clean_{chambername}", $"State", $"[{chambername}] Rotational Speed : {(int)currentRpm} rpm");
                    }
                    else
                    {
                        this._logManager.WriteLog($"Dry_{chambername}", $"State", $"[{chambername}] Rotational Speed : {(int)currentRpm} rpm");
                    }

                    await Task.Delay(500);
                }

                return currentRpm;
            }
            catch (Exception ex)
            {
                this._messageBox.Show("예외발생", ex.ToString());

                return 0;
            }
        }

        public async Task Stop(string chambername, bool bClean)
        {
            try
            {
                byte Chamber_Write_Register = bClean ? Get_CleanRegisters(chambername) : Get_DryRegisters(chambername);
                byte Chamber_Read_Register = (byte)(Chamber_Write_Register + 1);

                ushort targetRpm = 0;
                await _master.WriteSingleRegisterAsync(1, Chamber_Write_Register, targetRpm); // 목표 RPM 전송
                await _master.WriteSingleCoilAsync(Chamber_Write_Register, true);

                ushort[] values = await _master.ReadHoldingRegistersAsync(1, Chamber_Read_Register, 1); // 현재 RPM 읽기
                ushort currentRpm = values[0];

                // 목표 RPM 도달할 때까지 반복
                while (currentRpm > targetRpm)
                {
                    values = await _master.ReadHoldingRegistersAsync(1, Chamber_Read_Register, 1); // 현재 RPM 읽기
                    currentRpm = values[0];

                    if (bClean)
                    {
                        this._logManager.WriteLog($"Clean_{chambername}", $"State", $"{chambername}] Rotational Speed : {(int)currentRpm} rpm");
                    }
                    else
                    {
                        this._logManager.WriteLog($"Dry_{chambername}", $"State", $"[{chambername}] Rotational Speed : {(int)currentRpm} rpm");
                    }

                    await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                this._messageBox.Show("예외발생", ex.ToString());
                Console.WriteLine(ex.ToString());
            }
        }
    }
    #endregion
}
