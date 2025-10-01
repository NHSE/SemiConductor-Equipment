using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.interfaces;
using static SemiConductor_Equipment.Enums.PIOSignalEnum;

namespace SemiConductor_Equipment.Services
{
    public class OHTHandlerService : IOHTManager
    {
        #region FIELDS
        private TcpListener _server;
        private CancellationTokenSource? _cts;
        public bool _isRunning {  get; set; }
        public event EventHandler<List<int>> Insert_Wafer;
        public event Action<int> Remove_Wafer;
        public bool _isWafer { get; set; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public OHTHandlerService()
        {
            _isRunning = false;
            Initalize();
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public void Initalize()
        {
            int port = 503;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            _server = new TcpListener(localAddr, port);

            _server.Start();

            _isRunning = true;
            _isWafer = false;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => _ = Server_Start(_cts.Token));
        }

        public async Task Server_Start(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // 클라이언트 연결 대기
                    TcpClient tcpClient = await _server.AcceptTcpClientAsync(token);
                    Console.WriteLine("클라이언트 연결됨");

                    // 클라이언트마다 별도 Task에서 처리
                    _ = Task.Run(() => HandleClientAsync(tcpClient, token), token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"서버 Accept 오류: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken token)
        {
            using (tcpClient)
            using (NetworkStream stream = tcpClient.GetStream())
            {
                byte[] buffer = new byte[1024];

                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        int nbytes = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                        if (nbytes == 0)
                        {
                            // 클라이언트가 연결을 끊음
                            Console.WriteLine("클라이언트 연결 종료 감지");
                            break;
                        }

                        byte type = buffer[0];
                        byte received = buffer[1];

                        await ProcessReceivedData(type, received, buffer, nbytes, stream);
                    }
                }
                catch (IOException ex) when (ex.InnerException is SocketException sockEx && sockEx.SocketErrorCode == SocketError.ConnectionReset)
                {
                    Console.WriteLine("클라이언트 강제 종료 감지");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"클라이언트 처리 중 오류: {ex.Message}");
                }
            }
        }

        private async Task ProcessReceivedData(byte type, byte received, byte[] buffer, int nbytes, NetworkStream stream)
        {
            if (type == (byte)TransferAction.Load)
            {
                if (_isWafer)
                {
                    // 이미 존재
                }

                if (received == (byte)PioSignal.TR_REQ)
                {
                    var response = new List<byte> { type, (byte)PioSignal.READY };
                    await stream.WriteAsync(response.ToArray(), 0, response.Count);
                }
                else if (received == (byte)PioSignal.BUSY)
                {
                    byte Loadport_Number = buffer[2];

                    List<int> selectedWafers = new List<int>();
                    if (nbytes > 3)
                    {
                        for (int i = 3; i < nbytes; i++)
                            selectedWafers.Add(buffer[i]);
                    }

                    Insert_Wafer?.Invoke(this, selectedWafers);

                    var response = new List<byte> { type, (byte)PioSignal.L_REQ };
                    await stream.WriteAsync(response.ToArray(), 0, response.Count);
                }
                else
                {
                    var response = new List<byte> { type, Get_Response(received) };
                    await stream.WriteAsync(response.ToArray(), 0, response.Count);
                }
            }
            else // UnLoad
            {
                if (_isWafer)
                {
                    // 이미 없음
                }

                if (received == (byte)PioSignal.TR_REQ)
                {
                    var response = new List<byte> { type, (byte)PioSignal.READY };
                    await stream.WriteAsync(response.ToArray(), 0, response.Count);
                }
                else if (received == (byte)PioSignal.BUSY)
                {
                    byte Loadport_Number = buffer[2];
                    Remove_Wafer?.Invoke(Loadport_Number);

                    var response = new List<byte> { type, (byte)PioSignal.L_REQ };
                    await stream.WriteAsync(response.ToArray(), 0, response.Count);
                }
                else
                {
                    var response = new List<byte> { type, Get_Response(received) };
                    await stream.WriteAsync(response.ToArray(), 0, response.Count);
                }
            }

            await Task.Delay(1000); // 기존 지연 유지
        }

        private byte Get_Response(byte received)
        {
            byte ret = 0;
            switch (received)
            {
                case (byte)PioSignal.VALID:
                    ret = (byte)PioSignal.L_REQ;
                    //L_Req ON
                    break;

                case (byte)PioSignal.TR_REQ:
                    ret = (byte)PioSignal.READY;
                    //Ready ON
                    break;

                case (byte)PioSignal.READY:
                    ret = (byte)PioSignal.READY;
                    break;

                case (byte)PioSignal.COMPT:
                    ret = (byte)PioSignal.READY;
                    //Ready OFF
                    break;
            }

            return ret;
        }

        public void Stop()
        {
            // 1. CancellationTokenSource를 통해 비동기 작업 취소 요청
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
        #endregion
    }
}
