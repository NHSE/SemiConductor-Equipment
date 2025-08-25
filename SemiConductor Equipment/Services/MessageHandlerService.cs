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
using SemiConductor_Equipment.Enums;
using System.Timers;

namespace SemiConductor_Equipment.Services
{
    public class MessageHandlerService
    {
        #region FIELDS
        private readonly ILogManager _logManager;
        private readonly IEventMessageManager _eventMessageManager;
        private readonly IVIDManager _vIDManager;
        private readonly IAlarmMsgManager _alarmMsgManager;
        private readonly IMessageBox _messageBoxManager;
        private readonly ITraceDataManager _traceDataManager;
        private readonly WaferService _waferService;
        private readonly IWaferProcessCoordinator _processManager;
        private readonly LoadPortService _loadPortService;
        private readonly RunningStateService _runningStateService;

        private readonly Func<byte, ILoadPortViewModel> _loadPortFactory; // 팩토리 디자인 (대리자로 키, value값을 서비스 등록 때 전달받은 후 사용)

        private readonly Action<string> _logAction;

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
        public MessageHandlerService(ILogManager logManager, Action<string> logAction, Func<byte, ILoadPortViewModel> loadPortFactory, ITraceDataManager traceDataManager,
            WaferService waferService, IWaferProcessCoordinator processManager, LoadPortService loadPortService, IEventMessageManager eventMessageManager,
            IVIDManager vIDManager, IAlarmMsgManager alarmMsgManager, IMessageBox messageBoxManager, RunningStateService runningStateService)
        {
            this._logManager = logManager;
            this._logAction = logAction;
            this._loadPortFactory = loadPortFactory;
            this._waferService = waferService;
            this._processManager = processManager;
            this._loadPortService = loadPortService;
            this._eventMessageManager = eventMessageManager;
            this._vIDManager = vIDManager;
            this._alarmMsgManager = alarmMsgManager;
            this._messageBoxManager = messageBoxManager;
            this._runningStateService = runningStateService;
            this._traceDataManager = traceDataManager;
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

            else if(msg.S == 2 && msg.F == 23)
            {
                await TraceData(msg, wrapper, recv_logMessage);
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
                                _vIDManager.SetDVID(1009, pjId, (int)loadportId);
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
                await CJCreate(msg, wrapper, recv_logMessage, send_logMessage);
            }

            else if (msg.S == 2 && msg.F == 33) // CJ Create //수정 필
            {
                await HandleRPTIDVIDLink(msg, wrapper, recv_logMessage);
            }
            else if (msg.S == 2 && msg.F == 35) // CJ Create //수정 필
            {
                await HandleCEIDLink(msg, wrapper, recv_logMessage);
            }
            else if (msg.S == 2 && msg.F == 37) // CJ Create //수정 필
            {
                await HandleCEIDEnable(msg, wrapper, recv_logMessage);
            }
        }

        private async Task TraceData(SecsMessage msg, PrimaryMessageWrapper wrapper, string recv_logMessage)
        {
            try
            {
                string TRID = null, DSPER = null;
                uint TOTSMP = 0, REPGSZ = 0;
                List<uint> VID = new List<uint>();

                if (msg.SecsItem[0] != null)
                    TRID = msg.SecsItem[0].GetString();

                if (msg.SecsItem[1] != null)
                {
                    DSPER = msg.SecsItem[1].GetString();

                    bool isValidFormat = System.Text.RegularExpressions.Regex.IsMatch(DSPER, @"^\d{6}$");

                    if (!isValidFormat)
                    {
                        goto error_time;
                    }

                    bool isValidTime = DateTime.TryParseExact(DSPER, "HHmmss",
                                            System.Globalization.CultureInfo.InvariantCulture,
                                            System.Globalization.DateTimeStyles.None,
                                            out var time);
                    if (!isValidTime)
                    {
                        if (msg.ReplyExpected)
                        {
                            goto error_time;
                        }
                    }
                }

                if (msg.SecsItem[2] != null)
                    TOTSMP = msg.SecsItem[2].FirstValue<uint>();

                if (msg.SecsItem[3] != null)
                    REPGSZ = msg.SecsItem[3].FirstValue<uint>();


                int size = msg.SecsItem[4].Count;
                for (int cnt = 0; cnt < size; cnt++)
                {
                    uint vid = msg.SecsItem[4][cnt].FirstValue<uint>();
                    VID.Add(vid);
                }

                if(!this._traceDataManager.SetTraceData(TRID, DSPER, TOTSMP, REPGSZ, VID))
                {
                    goto error_time;
                }
            }
            catch (Exception e)
            {
                if (msg.ReplyExpected)
                {
                    var reply = new SecsMessage(2, 36)
                    {
                        Name = "CEID LINK",
                        SecsItem = B(2)
                    };

                    await wrapper.TryReplyAsync(reply);
                    _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                }
            }

        error_time:
            if (msg.ReplyExpected)
            {
                var reply = new SecsMessage(2, 24)
                {
                    Name = "Trace Data",
                    SecsItem = B(3)
                };

                await wrapper.TryReplyAsync(reply);
                _logManager.WriteLog("SECS", "RECV", recv_logMessage);
            }
        }   

        private async Task CJCreate(SecsMessage msg, PrimaryMessageWrapper wrapper, string recv_logMessage, string send_logMessage)
        {
            if (this._alarmMsgManager.IsAlarm)
            {
                this._messageBoxManager.Show("예외 발생", "Alarm이 존재합니다.\nAlarm Clear 후 재 진행하세요.");
                goto error_msg;
            }

            if (this._runningStateService.Get_State() == EquipmentStatusEnum.Running)
            {
                this._alarmMsgManager.AlarmMessage_IN("Currently Testing this");
                goto error_msg;
            }

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

            if (cmd != "ControlJob")
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
                    if (!viewModel.Check_Running(cjId))
                    {
                        continue;
                    }
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
                            _vIDManager.SetDVID(1010, cjId, (int)loadportId);
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
                            this._logManager.SetTime(DateTime.Now.ToString("yyyyMMddss_HHmmss"));
                            await _processManager.StartProcessAsync(_waferService.GetQueue(), cancellationToken);
                        }
                    });
                }
            }

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

                _vIDManager.SetDVID(1008, carrierId, (int)loadportId);
            }
        }

        private async Task HandleRPTIDVIDLink(SecsMessage msg, PrimaryMessageWrapper wrapper, string recv_logMessage)
        {
            try
            {
                string recv_log = recv_logMessage + msg.ToSml();
                _logManager.WriteLog("SECS", "RECV", recv_log);

                int rptid_cnt = msg.SecsItem[1].Count;
                bool flag = false;

                for (int i = 0; i < rptid_cnt; i++)
                {
                    uint rptid = msg.SecsItem[1][i][0].FirstValue<uint>();

                    if (this._eventMessageManager.IsRPTID(rptid))
                    {
                        if (msg.ReplyExpected)
                        {
                            var reply = new SecsMessage(2, 34)
                            {
                                Name = "RPTIDLINK",
                                SecsItem = B(3)
                            };

                            await wrapper.TryReplyAsync(reply);
                            _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                        }
                    }
                    else
                    {
                        int vid_cnt = msg.SecsItem[1][i][1].Count;
                        List<uint> vid_list = new List<uint>();

                        for (int j = 0; j < vid_cnt; j++)
                        {
                            uint vid = msg.SecsItem[1][i][1][j].FirstValue<uint>();

                            if (!this._eventMessageManager.IsVID(vid))
                            {
                                flag = true;
                                break;
                            }
                            else
                            {
                                vid_list.Add(vid);
                            }
                        }

                        if (flag)
                        {
                            break;
                        }
                        else
                        {
                            this._eventMessageManager.CreateRPTID(rptid, vid_list);
                        }
                    }
                }
                if (flag)
                {
                    if (msg.ReplyExpected)
                    {
                        var reply = new SecsMessage(2, 34)
                        {
                            Name = "RPTIDLINK",
                            SecsItem = B(4)
                        };

                        await wrapper.TryReplyAsync(reply);
                        _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                    }
                }
                else
                {
                    if (msg.ReplyExpected)
                    {
                        var reply = new SecsMessage(2, 34)
                        {
                            Name = "RPTIDLINK",
                            SecsItem = B(0)
                        };

                        await wrapper.TryReplyAsync(reply);
                        _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                    }
                }
            }
            catch (Exception e)
            {
                if (msg.ReplyExpected)
                {
                    var reply = new SecsMessage(2, 34)
                    {
                        Name = "RPTIDLINK",
                        SecsItem = B(2)
                    };

                    await wrapper.TryReplyAsync(reply);
                    _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                }
            }
        }

        private async Task HandleCEIDLink(SecsMessage msg, PrimaryMessageWrapper wrapper, string recv_logMessage)
        {
            try
            {
                string recv_log = recv_logMessage + msg.ToSml();
                _logManager.WriteLog("SECS", "RECV", recv_log);

                int ceid_cnt = msg.SecsItem[1].Count;
                bool flag = false;

                for (int i = 0; i < ceid_cnt; i++)
                {
                    uint ceid = msg.SecsItem[1][i][0].FirstValue<uint>();

                    if (!this._eventMessageManager.IsCEID(ceid))
                    {
                        if (msg.ReplyExpected)
                        {
                            var reply = new SecsMessage(2, 36)
                            {
                                Name = "CEID LINK",
                                SecsItem = B(4)
                            };

                            await wrapper.TryReplyAsync(reply);
                            _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                        }
                    }
                    else
                    {
                        int vid_cnt = msg.SecsItem[1][i][1].Count;
                        List<uint> rptid_list = new List<uint>();

                        for (int j = 0; j < vid_cnt; j++)
                        {
                            uint rptid = msg.SecsItem[1][i][1][j].FirstValue<uint>();

                            if (this._eventMessageManager.IsRPTIDInCEID(ceid, rptid))
                            {
                                flag = true;
                                break;
                            }
                            else
                            {
                                rptid_list.Add(rptid);
                            }
                        }

                        if (flag)
                        {
                            break;
                        }
                        else
                        {
                            this._eventMessageManager.LinkCEID(ceid, rptid_list);
                        }
                    }
                }
                if (flag)
                {
                    if (msg.ReplyExpected)
                    {
                        var reply = new SecsMessage(2, 36)
                        {
                            Name = "CEID LINK",
                            SecsItem = B(5)
                        };

                        await wrapper.TryReplyAsync(reply);
                        _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                    }
                }
                else
                {
                    if (msg.ReplyExpected)
                    {
                        var reply = new SecsMessage(2, 36)
                        {
                            Name = "CEID LINK",
                            SecsItem = B(0)
                        };

                        await wrapper.TryReplyAsync(reply);
                        _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                    }
                }
            }
            catch (Exception e)
            {
                if (msg.ReplyExpected)
                {
                    var reply = new SecsMessage(2, 36)
                    {
                        Name = "CEID LINK",
                        SecsItem = B(2)
                    };

                    await wrapper.TryReplyAsync(reply);
                    _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                }
            }
        }

        private async Task HandleCEIDEnable(SecsMessage msg, PrimaryMessageWrapper wrapper, string recv_logMessage)
        {
            try
            {
                string recv_log = recv_logMessage + msg.ToSml();
                _logManager.WriteLog("SECS", "RECV", recv_log);

                bool State = msg.SecsItem[0].FirstValue<bool>();
                int ceid_cnt = msg.SecsItem[1].Count;
                List<uint> ceid_list = new List<uint>();
                if (ceid_cnt > 0)
                {
                    bool flag = false;
                    for (int i = 0; i < ceid_cnt; i++)
                    {
                        uint ceid = msg.SecsItem[1][i].FirstValue<uint>();

                        if (!this._eventMessageManager.IsCEID(ceid))
                        {
                            throw new Exception();
                        }
                        else
                        {
                            ceid_list.Add(ceid);
                        }
                    }
                }
                else if (ceid_cnt == 0)
                {
                    this._eventMessageManager.CEIDStateChange(0, State);
                }
                else throw new Exception();

                if (ceid_list.Count > 0)
                {
                    foreach (uint ceid in ceid_list)
                    {
                        this._eventMessageManager.CEIDStateChange((int)ceid, State);
                    }

                    if (msg.ReplyExpected)
                    {
                        var reply = new SecsMessage(2, 38)
                        {
                            Name = "Enable/Disable Event",
                            SecsItem = B(0)
                        };

                        await wrapper.TryReplyAsync(reply);
                        _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                    }
                }
            }
            catch (Exception e)
            {
                if (msg.ReplyExpected)
                {
                    var reply = new SecsMessage(2, 38)
                    {
                        Name = "Enable/Disable Event",
                        SecsItem = B(1)
                    };

                    await wrapper.TryReplyAsync(reply);
                    _logManager.WriteLog("SECS", "RECV", recv_logMessage);
                }
            }
        }
        #endregion
    }
}
