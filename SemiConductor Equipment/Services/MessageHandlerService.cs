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

namespace SemiConductor_Equipment.Services
{
    public class MessageHandlerService
    {
        #region FIELDS
        private readonly ILogManager _logManager;
        private readonly Action<string> _logAction;
        private readonly Func<byte, ILoadPortViewModel> _loadPortFactory; // 팩토리 디자인 (대리자로 키, value값을 서비스 등록 때 전달받은 후 사용)
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public MessageHandlerService(ILogManager logManager, Action<string> logAction, Func<byte, ILoadPortViewModel> loadPortFactory)
        {
            _logManager = logManager;
            _logAction = logAction;
            _loadPortFactory = loadPortFactory;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public async Task HandleMessageAsync(PrimaryMessageWrapper wrapper)
        {
            var msg = wrapper.PrimaryMessage;
            string recv_logMessage = $"[RECV] S{msg.S}F{msg.F} (W? {msg.ReplyExpected})";
            string send_logMessage = $"[SEND] → S{msg.S}F{msg.F + 1} (ACK 응답)";

            // S1F1 처리
            if (msg.S == 1 && msg.F == 1)
            {
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
                _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                _logAction?.Invoke(recv_logMessage);
            }

            // S2F21 처리 (FDC 요청)
            else if (msg.S == 2 && msg.F == 21)
            {
                Item cmd = msg.SecsItem;

                // Remote Command 수락 응답 (S2F22)
                var reply = new SecsMessage(2, 22, false);
                await wrapper.TryReplyAsync(reply);
                //여기서 데이터 파싱 후
                //S9F1로 설정된 타임마다 데이터 전송


                // TODO: FDC 데이터 전송 로직은 여기에 추가
                _logManager.WriteLog("SECS", "RECV", $"{recv_logMessage} → RemoteCommand: {cmd}");
                _logAction?.Invoke($"{recv_logMessage} → RemoteCommand: {cmd}");
            }

            else if (msg.S == 3 && msg.F == 17)
            {
                // S3F17: 웨이퍼 정보 수신
                string? cmd = msg?.SecsItem?[1].GetString();
                if (cmd == "ProceedWithCarrier")
                {
                    string? carrierId = msg?.SecsItem?[2].GetString();
                    byte loadportId = msg.SecsItem[3].FirstValue<byte>();
                    bool success = false;

                    var viewModel = _loadPortFactory(loadportId);
                    if (viewModel != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            success = viewModel.Update_Carrier_info(carrierId);
                        });
                    }

                    if (success)
                    {
                        // S3F18 응답 (ACK)
                        var reply = new SecsMessage(3, 18)
                        {
                            Name = "ProceedWithCarrier",
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
                        _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                        _logAction?.Invoke(recv_logMessage);
                    }
                    else
                    {
                        var reply = new SecsMessage(3, 18)
                        {
                            Name = "Nothing",
                            SecsItem = L(
                                        U1(0),
                                        L(
                                            L(
                                                U4(1),
                                                A("unknown object")
                                              )
                                          )
                                     )
                        };
                        await wrapper.TryReplyAsync(reply);
                    }
                }
                else
                {
                    var reply = new SecsMessage(3, 18)
                    {
                        Name = "Nothing",
                        SecsItem = L(
                                        U1(0),
                                        L(
                                            L(
                                                U4(1),
                                                A("unknown object")
                                              )
                                          )
                                     )
                    };
                    await wrapper.TryReplyAsync(reply);
                }
            }

            else if (msg.S == 16 && msg.F == 11)
            {
                // S3F17: 웨이퍼 정보 수신
                string? cmd = msg?.SecsItem?[1].GetString();

                string? carrierId = msg?.SecsItem?[2].GetString();
                byte loadportId = msg.SecsItem[3].FirstValue<byte>();

                var waferService = App.Services.GetRequiredService<WaferService>();

                // S3F18 응답 (ACK)
                var reply = new SecsMessage(16, 12)
                {
                    Name = "ProceedWithCarrier",
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
            }


            // W-bit 응답
            if (msg.ReplyExpected)
            {
                var ack = new SecsMessage(msg.S, (byte)(msg.F + 1), true);
                await wrapper.TryReplyAsync(ack);
                _logManager.WriteLog("SECS", "SEND", send_logMessage);
                _logAction?.Invoke(send_logMessage);

            }
        }
        #endregion
    }
}
