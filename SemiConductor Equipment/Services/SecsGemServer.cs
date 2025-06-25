using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Options;
using Secs4Net;

namespace SemiConductor_Equipment.Services
{
    public class SecsGemServer
    {
        private readonly Action<string> _log;
        private readonly SecsGem _secs;
        private ISecsConnection _hsmsConnector;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        private readonly MessageHandlerService _messageHandler;
        private CancellationTokenSource? _cts;

        public static SecsGemServer Instance { get; private set; }

        public SecsGemServer(Action<string> logger, MessageHandlerService messageHandler, string ipAddress, int port, ushort deviceId)
        {
            _log = logger;
            _messageHandler = messageHandler;

            // 1) SecsGemOptions 생성 (receiveBufferSize 역할을 하는 옵션 추가)
            var secsGemOptions = Options.Create(new SecsGemOptions
            {
                IpAddress = ipAddress, // 서버 IP
                Port = port,                 // 시뮬레이터에서 사용하는 포트
                DeviceId = deviceId,
                T3 = 5000,
                EncodeBufferInitialSize = 4096,
                SocketReceiveBufferSize = 16384,
                IsActive = false
            });

            // 2) 로거 구현체 생성
            ISecsGemLogger secsLogger = new ActionLogger(_log);

            // 3) HsmsConnection 생성자에 옵션과 로거 전달
            this._hsmsConnector = new HsmsConnection(secsGemOptions, secsLogger);

            // 4) SecsGem 생성
            _secs = new SecsGem(secsGemOptions, this._hsmsConnector, secsLogger);

            // 이벤트 등록 (PrimaryMessageWrapper)
            this._hsmsConnector.ConnectionChanged += OnConnectionChanged;
        }

        public static void Initialize(Action<string> logger, MessageHandlerService messageHandler, string ipAddress, int port, ushort deviceId)
        {
            if (Instance == null)
            {
                Instance = new SecsGemServer(logger, messageHandler, ipAddress, port, deviceId);
                Instance.Start();
            }
        }

        public void Start()
        {
            this._hsmsConnector.Start(CancellationToken.None);
            _log("[START] SECS/GEM 서버 시작됨 (Passive Mode)");

            _cts = new CancellationTokenSource();
            Task.Run(() => ReceivePrimaryMessagesAsync(_cts.Token));
        }

        private void OnConnected() => Connected?.Invoke(this, EventArgs.Empty);
        private void OnDisconnected() => Disconnected?.Invoke(this, EventArgs.Empty);

        private void OnConnectionChanged(object sender, ConnectionState e)
        {
            switch (e)
            {
                case ConnectionState.Connecting:
                    _log("[CONNECTING] 연결 시도 중...");
                    break;
                case ConnectionState.Connected:
                    _log("[CONNECTED] 호스트 연결됨");
                    OnConnected();
                    break;
                case ConnectionState.Selected:
                    _log("[SELECTED] 통신 세션 선택됨");
                    break;
                case ConnectionState.Retry:
                    _log("[DISCONNECTING] 호스트 연결 해제됨...");
                    OnDisconnected();
                    // 재접속 로직 구현
                    break;
                    // Disconnecting, Disconnected는 없음
            }
        }

        private async Task ReceivePrimaryMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var wrapper in _secs.GetPrimaryMessageAsync(cancellationToken).ConfigureAwait(false))
                {
                    await _messageHandler.HandleMessageAsync(wrapper);
                }
            }
            catch (OperationCanceledException)
            {
                _log("[INFO] Primary message receiving cancelled.");
            }
            catch (Exception ex)
            {
                _log($"[ERROR] Exception in ReceivePrimaryMessagesAsync: {ex}");
            }
        }
    }
}
