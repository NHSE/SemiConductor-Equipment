using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Options;
using Secs4Net;
using Secs4Net.Sml;
using SemiConductor_Equipment.interfaces;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.Services
{
    public class SecsGemServer : ISecsGemServer
    {
        private Action<string> _log;
        private SecsGem _secs;
        private ISecsConnection _hsmsConnector;
        private IEventMessageManager _eventMessageManager;
        private readonly IAlarmMsgManager _alarmMsgManager;
        private readonly ITraceDataManager _traceDataManager;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        private readonly MessageHandlerService _messageHandler;
        private CancellationTokenSource? _cts;
        private CancellationTokenSource _connectingCts;

        public SecsGemServer(Action<string> logger, MessageHandlerService messageHandler, IEventMessageManager eventMessageManager, 
            IAlarmMsgManager alarmMsgManager, ITraceDataManager traceDataManager)
        {
            this._messageHandler = messageHandler;
            this._eventMessageManager = eventMessageManager;
            this._alarmMsgManager = alarmMsgManager;
            this._traceDataManager = traceDataManager;
        }

        public bool Initialize(Action<string> logger, MessageHandlerService messageHandler, IConfigManager configManager)
        {
            bool ret = false;

            if (this._hsmsConnector == null)
            {
                _log = logger;
                var secsGemOptions = Options.Create(new SecsGemOptions
                {
                    IpAddress = configManager.IP, // 서버 IP
                    Port = configManager.Port,                 // 시뮬레이터에서 사용하는 포트
                    DeviceId = configManager.DeviceID,
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

                _eventMessageManager.SetSecsGem(_secs);
                _eventMessageManager.SetConnect(this._hsmsConnector);

                _traceDataManager.SetSecsGem(_secs);
                _traceDataManager.SetConnect(this._hsmsConnector);

                this._hsmsConnector.ConnectionChanged += OnConnectionChanged;
                Start();
                ret = true;
            }
            else
            {
                Stop();
                ret = true;
            }
                return ret;
        }

        public void Start()
        {
            this._hsmsConnector.Start(CancellationToken.None);
            _log("[START] SECS/GEM 서버 시작됨 (Passive Mode)");

            _cts = new CancellationTokenSource();
            Task.Run(() => ReceivePrimaryMessagesAsync(_cts.Token));
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
            
            if (_hsmsConnector is HsmsConnection disposable)
            {
                disposable.DisposeAsync();
                this._hsmsConnector = null;
            }

            // 3. 로그 기록
            _log("[STOP] SECS/GEM 서버 종료됨");
            this._eventMessageManager.DisConnect();
        }

        private void OnConnected() => Connected?.Invoke(this, EventArgs.Empty);
        private void OnDisconnected() => Disconnected?.Invoke(this, EventArgs.Empty);

        private void OnConnectionChanged(object sender, ConnectionState e)
        {
            switch (e)
            {
                case ConnectionState.Connecting:
                    _log("[CONNECTING] 연결 시도 중...");

                    /// 이전 타이머 취소
                    _connectingCts?.Cancel();
                    _connectingCts = new CancellationTokenSource();
                    var token = _connectingCts.Token;

                    // 백그라운드에서 5초 감시
                    Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(5000, token);
                            _alarmMsgManager?.AlarmMessage_IN("SECS/GEM communication failure detected.");
                        }
                        catch (TaskCanceledException) { }
                    });
                    break;
                case ConnectionState.Connected:
                    _log("[CONNECTED] 호스트 연결됨");
                    _connectingCts?.Cancel();
                    OnConnected();
                    break;
                case ConnectionState.Selected:
                    _log("[SELECTED] 통신 세션 선택됨");
                    _connectingCts?.Cancel();
                    break;
                case ConnectionState.Retry:
                    _log("[DISCONNECTING] 호스트 연결 해제됨...");
                    OnDisconnected();
                    // 재접속 로직 구현
                    break;
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
                _alarmMsgManager?.AlarmMessage_IN("[INFO] Primary message receiving cancelled.");
            }
            catch (Exception ex)
            {
                _alarmMsgManager?.AlarmMessage_IN($"[ERROR] Exception in ReceivePrimaryMessagesAsync: {ex}");
            }
        }
    }
}
