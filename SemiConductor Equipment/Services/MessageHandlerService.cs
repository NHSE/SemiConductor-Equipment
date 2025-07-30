using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;
using static Secs4Net.Item;
using SemiConductor_Equipment.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using SemiConductor_Equipment.ViewModels.Pages;
using SemiConductor_Equipment.Models;
using System.Threading;
using System.Data;
using static SemiConductor_Equipment.Models.EventInfo;
using System.Windows.Interop;
using Secs4Net.Sml;

namespace SemiConductor_Equipment.Services
{
    public class MessageHandlerService
    {
        #region FIELDS
        private readonly ILogManager _logManager;
        private readonly IEventMessageManager _eventMessageManager;
        private readonly Action<string> _logAction;
        private readonly Func<byte, ILoadPortViewModel> _loadPortFactory; // 팩토리 디자인 (대리자로 키, value값을 서비스 등록 때 전달받은 후 사용)
        private readonly WaferService _waferService;
        private readonly WaferProcessCoordinatorService _coordinator;
        private readonly LoadPortService _loadPortService;
        private readonly RunningStateService _runningStateService;

        string? cmd;
        string? cjId;
        string? carrier_cmd;
        string? carrierId;
        string? pj_cmd;
        string? pjId;
        string? auto_start;
        bool auto_start_flag;
        byte loadportId;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public MessageHandlerService(ILogManager logManager, Action<string> logAction, Func<byte, ILoadPortViewModel> loadPortFactory,
            WaferService waferService, WaferProcessCoordinatorService coordinator, LoadPortService loadPortService, IEventMessageManager eventMessageManager)
        {
            _logManager = logManager;
            _logAction = logAction;
            _loadPortFactory = loadPortFactory;
            _waferService = waferService;
            _coordinator = coordinator;
            _loadPortService = loadPortService;
            _eventMessageManager = eventMessageManager;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public async Task HandleMessageAsync(PrimaryMessageWrapper wrapper)
        {
            var msg = wrapper.PrimaryMessage;
            string recv_logMessage = $"[RECV] S{msg.S}F{msg.F} (W? {msg.ReplyExpected})\n";
            string send_logMessage = $"[SEND] → S{msg.S}F{msg.F + 1} (ACK 응답)\n";

            // S1F1 처리
            if (msg.S == 1 && msg.F == 1)
            {
                string recv_log = recv_logMessage + msg.ToSml();
                _logManager.WriteLog("SECS", "RECV", recv_log);

                var reply = new SecsMessage(1, 2)
                {
                    Name = "CreateProcessJob",
                    SecsItem = L(
                                    U4(0),
                                    L(
                                        L(
                                            A("Id"),
                                            B(0x0D),
                                            L(
                                                A("carrier id"),
                                                L(
                                                    U1(1)),
                                                L(
                                                    U1(1),
                                                    A("recipe"),
                                                    L()),
                                                Boolean(true),
                                                L()))))
                };
                await wrapper.TryReplyAsync(reply);
                string send_log = send_logMessage + reply.ToSml();
                _logManager.WriteLog("SECS", "SEND", send_log);
            }

            else if (msg.S == 3 && msg.F == 17)
            {
                // S3F17: 웨이퍼 정보 수신
                if (msg?.SecsItem?[1] != null)
                    cmd = msg?.SecsItem?[1].GetString();
                else
                    return;
                if (cmd == "ProceedWithCarrier")
                {
                    await HandleProceedWithCarrier(msg, wrapper, recv_logMessage);
                }
            }

            else if (msg.S == 16 && msg.F == 11)
            {
                // S3F17: 웨이퍼 정보 수신
                if (msg.SecsItem[1] != null)
                    pjId = msg?.SecsItem?[1].GetString();
                else
                    return;

                if (msg?.SecsItem?[3][0][0] != null)
                    carrierId = msg?.SecsItem?[3][0][0].GetString();
                else
                    return;

                string recv_log = recv_logMessage + msg.ToSml();
                _logManager.WriteLog("SECS", "RECV", recv_log);

                bool success = false;

                for (byte loadportId = 1; loadportId <= 2; loadportId++)
                {
                    var viewModel = _loadPortFactory(loadportId);
                    if (viewModel != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (carrierId == viewModel.GetCarrierId())
                            {
                                var waferData = new Wafer
                                {
                                    PJId = pjId
                                };
                                success = viewModel.Update_Carrier_info(waferData);
                            }
                        });
                    }
                }

                if (success && msg.ReplyExpected)
                {
                    // S3F18 응답 (ACK)
                    var reply = new SecsMessage(16, 12)
                    {
                        Name = "PJCreate",
                        SecsItem = L(
                                        U1(0),
                                        L(
                                            L(
                                                U4(0),
                                                A("no error")
                                              )
                                          )
                                     )
                    };
                    await wrapper.TryReplyAsync(reply);
                    string send_log = send_logMessage + reply.ToSml();
                    _logManager.WriteLog("SECS", "SEND", send_log);
                }
                else if (msg.ReplyExpected && !success)
                {
                    var reply = new SecsMessage(16, 12)
                    {
                        Name = "test",
                        SecsItem = L(
                                    U1(0),
                                    L(
                                        L(
                                            U4(0),
                                            A("error")
                                          )
                                      )
                                 )
                    };
                    await wrapper.TryReplyAsync(reply);
                    string send_log = send_logMessage + reply.ToSml();
                    _logManager.WriteLog("SECS", "SEND", send_log);
                }

            }
            else if (msg.S == 14 && msg.F == 9)// CJ Create //수정 필
            {
                if (msg.SecsItem[1] != null)
                    cmd = msg?.SecsItem?[1].GetString();
                else goto error_msg;

                if (msg.SecsItem[2][0][1] != null)
                    cjId = msg?.SecsItem?[2][0][1].GetString();
                else goto error_msg;

                if (msg.SecsItem[2][1][0] != null)
                    carrier_cmd = msg?.SecsItem?[2][1][0].GetString();
                else goto error_msg;

                if (msg.SecsItem[2][1][1][0] != null)
                    carrierId = msg?.SecsItem?[2][1][1][0].GetString();
                else goto error_msg;

                if (msg.SecsItem[2][3][0] != null)
                    pj_cmd = msg?.SecsItem?[2][3][0].GetString();
                else goto error_msg;

                if (msg.SecsItem[2][3][1][0][0] != null)
                    pjId = msg?.SecsItem?[2][3][1][0][0].GetString();
                else goto error_msg;

                if (msg.SecsItem[2][4][0] != null)
                    auto_start = msg?.SecsItem?[2][4][0].GetString();
                else goto error_msg;

                //bool auto_start_flag = msg?.SecsItem?[2][1][4][1].FirstValue<bool>;

                if(cmd != "ControlJob")
                {
                    goto error_msg;
                }

                string recv_log = recv_logMessage + msg.ToSml();
                _logManager.WriteLog("SECS", "RECV", recv_log);

                for (byte loadportId = 1; loadportId <= 2; loadportId++)
                {
                    var viewModel = _loadPortFactory(loadportId);
                    if (viewModel != null)
                    {
                        // Dispatcher.InvokeAsync를 사용하여 비동기 처리
                        await Application.Current.Dispatcher.InvokeAsync(async () =>
                        {
                            if (viewModel.GetPJId(loadportId) == pjId)
                            {
                                var waferData = new Wafer
                                {
                                    CJId = cjId
                                };
                                viewModel.Update_Carrier_info(waferData);
                                // pjId로 해당 웨이퍼 리스트를 가져옴
                                var wafers = viewModel.GetAllWaferInfo(pjId);

                                // wafers가 null 또는 비어있는지 체크
                                if (wafers == null || wafers.Count == 0)
                                    return;

                                // LoadPortId로 초기화
                                _loadPortService.SetInitialWafers($"LoadPort{wafers[0].LoadportId}", wafers);

                                // 각 웨이퍼를 큐에 등록
                                foreach (var wafer_info in wafers)
                                {
                                    _waferService.Enqueue(wafer_info);
                                }
                                var cts = new CancellationTokenSource();
                                var cancellationToken = cts.Token;
                                _logManager.LogDataTime = DateTime.Now.ToString("yyyyMMddss_HHmmss");
                                await _coordinator.StartProcessAsync(_waferService.GetQueue(), cancellationToken);
                            }
                        });
                    }
                }
            }


            // W-bit 응답
            if (msg.ReplyExpected)
            {
                var reply = new SecsMessage(14, 10)
                {
                    Name = "ControlJob",
                    SecsItem = L(
                                    A(cmd),
                                    L(
                                       L(
                                           A("ControlJob"),
                                           A(cjId)
                                       )
                                    ),
                                    L(
                                        U4(0),
                                        L(
                                            L(
                                                U4(0),
                                                A("no error")
                                             )
                                         )
                                    )
                                )
                };
                string send_log = send_logMessage + reply.ToSml();
                _logManager.WriteLog("SECS", "SEND", send_log);
            }


        error_msg:
            var errormsg = new SecsMessage(14, 10)
            {
                Name = "ControlJob",
                SecsItem = L(
                                    A(cmd),
                                    L(
                                       L(
                                           A("ControlJob"),
                                           A(cjId)
                                       )
                                    ),
                                    L(
                                        U4(1),
                                        L(
                                            L(
                                                U4(2),
                                                A("unknown class")
                                             )
                                         )
                                    )
                                )
            };
            await wrapper.TryReplyAsync(errormsg);
            string send_errorlog = send_logMessage + errormsg.ToSml();
            _logManager.WriteLog("SECS", "SEND", send_errorlog);
        }

        private async Task HandleProceedWithCarrier(SecsMessage msg, PrimaryMessageWrapper wrapper, string recv_logMessage)
        {
            string? carrierId = msg?.SecsItem?[2]?.GetString();
            if (string.IsNullOrEmpty(carrierId))
                return;

            if (msg?.SecsItem?[3] == null)
                return;

            byte loadportId = msg.SecsItem[3].FirstValue<byte>();
            bool success = false;

            var waferData = new Wafer { CarrierId = carrierId };
            var viewModel = _loadPortFactory(loadportId);
            if (viewModel != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    success = viewModel.Update_Carrier_info(waferData);
                });
            }

            string recv_log = recv_logMessage + msg.ToSml();
            _logManager.WriteLog("SECS", "RECV", recv_log);

            if (msg.ReplyExpected)
            {
                var reply = new SecsMessage(3, 18)
                {
                    Name = success ? "ProceedWithCarrier" : "Nothing",
                    SecsItem = L(
                        U1(0    ),
                        L(
                            L(
                                U4((uint)(success ? 0 : 1)),
                                A(success ? "no error" : "unknown object")
                            )
                        )
                    )
                };

                await wrapper.TryReplyAsync(reply);
                _logManager.WriteLog("SECS", "RECV", recv_logMessage);
            }
        }
        #endregion
    }
}
