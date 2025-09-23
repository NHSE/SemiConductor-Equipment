using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;
using Secs4Net.Sml;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using static Secs4Net.Item;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.Services
{
    public class EventMessageService : IEventMessageManager
    {
        #region FIELDS
        private readonly IEventConfigManager _eventConfigManager;
        private readonly ILogManager _logManager;
        private readonly IVIDManager _vIDManager;
        private CancellationTokenSource _cts;
        private ISecsGem _secs;
        private readonly Queue<EventQueueItem> _EventQue = new();
        private ISecsConnection _connection;
        private bool IsActive = false;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// 발생된 이벤트를 호스트에게 전달하는 서비스 레이어
        /// </summary>
        /// <param name="eventConfigManager"></param>
        /// <param name="logManager"></param>
        /// <param name="VIDManager"></param>
        public EventMessageService(IEventConfigManager eventConfigManager, ILogManager logManager, IVIDManager VIDManager)
        {
            this._eventConfigManager = eventConfigManager;
            this._logManager = logManager;
            this._vIDManager = VIDManager;
        }

        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        /// <summary>
        /// CEID 정보 획득 메서드
        /// </summary>
        /// <param name="ceid_num"></param>
        /// <returns>CEID 정보</returns>
        public CEIDInfo GetCEID(int ceid_num)
        {
            this._eventConfigManager.InitCEIDConfig();
            return this._eventConfigManager.CEID[ceid_num];
        }

        /// <summary>
        /// CEID 존재 여부 확인 메서드
        /// </summary>
        /// <param name="ceid"></param>
        /// <returns>CEID 존재 여부</returns>

        public bool IsCEID(uint ceid)
        {
            return this._vIDManager.IsCEID(ceid);
        }

        /// <summary>
        /// RPTID가 CEID내에 있는 지 확인 메서드
        /// </summary>
        /// <param name="ceid"></param>
        /// <param name="rptid"></param>
        /// <returns>CEID 내 RPTID 존재 여부</returns>
        public bool IsRPTIDInCEID(uint ceid, uint rptid)
        {
            return this._vIDManager.IsRPTIDInCEID(ceid, rptid);
        }

        /// <summary>
        /// RPTID 존재 여부 메서드
        /// </summary>
        /// <param name="rptid"></param>
        /// <returns>RPTID 존재 여부</returns>
        public bool IsRPTID(uint rptid)
        {
            return this._vIDManager.IsRPTID(rptid);
        }

        /// <summary>
        /// VID 존재 여부 메서드
        /// </summary>
        /// <param name="vid"></param>
        /// <returns>VID 존재 여부</returns>
        public bool IsVID(uint vid)
        {
            if(!this._vIDManager.IsVID(vid))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// SECS/GEM으로 RPTID 생성하는 메서드
        /// </summary>
        /// <param name="rptid"></param>
        /// <param name="vid"></param>
        public void CreateRPTID(uint rptid, List<uint> vid)
        {
            RPTIDInfo Item = new RPTIDInfo();
            Item.Number = (int)rptid;
            Item.VIDs = new List<int>();

            foreach (uint i in vid)
            {
                Item.VIDs.Add((int)i);
            }

            this._eventConfigManager.CreatedRPTIDSectionPartial(Item);
        }

        /// <summary>
        /// SECS/GEM으로 CEID와 RPTID를 링크 시키는 메서드
        /// </summary>
        /// <param name="ceid"></param>
        /// <param name="rptid"></param>
        public void LinkCEID(uint ceid, List<uint> rptid)
        {
            CEIDInfo Item = this._eventConfigManager.CEID[(int)ceid];

            foreach (uint i in rptid)
            {
                Item.RPTIDs.Add((int)i);
            }

            this._eventConfigManager.UpdateCEIDSectionPartial(Item);
        }

        /// <summary>
        /// SECS/GEM으로 CEID 상태 변경
        /// </summary>
        /// <param name="ceid"></param>
        /// <param name="state"></param>
        public void CEIDStateChange(int ceid, bool state)
        {
            this._eventConfigManager.CEIDStateChange(ceid, state);
        }

        /// <summary>
        /// CEID의 상태를 확인하는 메서드
        /// </summary>
        /// <param name="ceid"></param>
        /// <returns>CEID 상태</returns>
        public bool IsCEIDEnabled(int ceid)
        {
            return this._eventConfigManager.CEID[ceid].State;
        }

        /// <summary>
        /// CEID 시점에 맞는 이벤트 발생
        /// </summary>
        /// <param name="eventData"></param>
        public void EnqueueEventData(CEIDInfo eventData)
        {
            var vidItems = new List<Item>();
            foreach (int rptid in eventData.RPTIDs)
            {
                if (eventData.Number != 100)
                    vidItems.AddRange(this._vIDManager.GetRPTID(rptid, eventData.Wafer_number, eventData.Loadport_Number));
                else
                    vidItems.AddRange(this._vIDManager.GetRPTID(rptid, eventData.Wafer_List, eventData.Loadport_Number));
            }

            EventQueueItem data = new EventQueueItem();
            data.CEID = eventData;
            data.VIDItems = vidItems;

            _EventQue.Enqueue(data);
        }

        public void SetSecsGem(ISecsGem secsGem) => _secs = secsGem;

        public void SetConnect(ISecsConnection connection)
        {
            _connection = connection;
            _connection.ConnectionChanged += OnState;
        }

        public void DisConnect()
        {
            IsActive = false;
        }

        private void OnState(object? sender, ConnectionState e)
        {
            if (e == ConnectionState.Selected)
                IsActive = true;
            else
                IsActive = false;

        }

        /// <summary>
        /// 호스트에게 이벤트 데이터를 전송시키는 메서드
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task ProcessEventQueueAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    EventQueueItem? eventData = null;

                    if (!IsActive) // 통신이 끊겼을 때
                    {
                        await Task.Delay(1000, token);
                        continue;
                    }

                    if (_EventQue.Count > 0)
                    {
                        eventData = _EventQue.Dequeue();

                        if (eventData == null) continue;

                        if (!IsCEIDEnabled(eventData.CEID.Number))
                            continue;

                        var eventmsg = new SecsMessage(6, 11, false)
                        {
                            Name = "Event Report Send",
                            SecsItem = L(
                               U4(0),
                               U4((uint)eventData.CEID.Number),
                               L(
                                   eventData.VIDItems.ToArray()
                                )
                            )
                        };

                        await _secs.SendAsync(eventmsg);
                        string send_logMessage = $"[SEND] → S6F11\n{eventmsg.ToSml()}";
                        _logManager.WriteLog("Event", "SEND", send_logMessage);
                    }
                    else
                    {
                        await Task.Delay(1000, token);
                        continue;
                    }
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// 이벤트 서비스 동작 실행 메서드
        /// </summary>
        public void StartProcessing()
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    await ProcessEventQueueAsync(token);
                }
                catch (Exception ex)
                {
                    // 예외 로깅
                    Console.WriteLine(ex.ToString());
                }
            });
        }

        /// <summary>
        /// 종료 시 이벤트 서비스 동작 종료 메서드
        /// </summary>
        /// <returns></returns>
        public async Task StopProcessing()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                while (_EventQue.Count() > 0)
                {
                    await Task.Delay(300); // 다른 Task에 영향 X
                }
                _cts.Cancel();
                _EventQue.Clear();
            }
        }
        #endregion

    }
}
