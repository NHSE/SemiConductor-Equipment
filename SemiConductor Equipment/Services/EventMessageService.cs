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
using static Secs4Net.Item;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.Services
{
    public partial class EventMessageService : IEventMessageManager
    {
        #region FIELDS
        private readonly IEventConfigManager _eventConfigManager;
        private readonly ILogManager _logManager;
        private readonly IVIDManager _vIDManager;
        private CancellationTokenSource _cts;
        private ISecsGem _secs;
        private readonly Queue<CEIDInfo> _EventQue = new();
        private ISecsConnection _connection;
        private bool IsActive = false;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
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
        public CEIDInfo GetCEID(int ceid_num)
        {
            this._eventConfigManager.InitCEIDConfig();
            return this._eventConfigManager.CEID[ceid_num];
        }

        public bool IsCEID(uint ceid)
        {
            return this._vIDManager.IsCEID(ceid);
        }

        public bool IsRPTIDInCEID(uint ceid, uint rptid)
        {
            return this._vIDManager.IsRPTIDInCEID(ceid, rptid);
        }

        public bool IsRPTID(uint rptid)
        {
            return this._vIDManager.IsRPTID(rptid);
        }

        public bool IsVID(uint rptid, uint vid)
        {
            if(!this._vIDManager.IsVID(rptid, vid))
            {
                return false;
            }

            return true;
        }

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

        public void LinkCEID(uint ceid, List<uint> rptid)
        {
            CEIDInfo Item = this._eventConfigManager.CEID[(int)ceid];

            foreach (uint i in rptid)
            {
                Item.RPTIDs.Add((int)i);
            }

            this._eventConfigManager.UpdateCEIDSectionPartial(Item);
        }

        public void CEIDStateChange(int ceid, bool state)
        {
            this._eventConfigManager.CEIDStateChange(ceid, state);
        }

        public bool IsCEIDEnabled(int ceid)
        {
            return this._eventConfigManager.CEID[ceid].State;
        }

        public void EnqueueEventData(CEIDInfo eventData)
        {
            _EventQue.Enqueue(eventData);
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

        public async Task ProcessEventQueueAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    CEIDInfo? eventData = null;

                    if (!IsActive) // 통신이 끊겼을 때
                    {
                        await Task.Delay(1000, token);
                        continue;
                    }

                    if (_EventQue.Count > 0)
                    {
                        eventData = _EventQue.Dequeue();

                        if (!IsCEIDEnabled(eventData.Number))
                            continue;

                        var vidItems = new List<Item>();
                        foreach (int rptid in eventData.RPTIDs)
                        {
                            if(eventData.Number != 100)
                                vidItems.AddRange(this._vIDManager.GetRPTID(rptid, eventData.Wafer_number, eventData.Loadport_Number));
                            else
                                vidItems.AddRange(this._vIDManager.GetRPTID(rptid, eventData.Wafer_List, eventData.Loadport_Number));
                        }

                        var eventmsg = new SecsMessage(6, 11, false)
                        {
                            Name = "Event Report Send",
                            SecsItem = L(
                               U4(0),
                               U4((uint)eventData.Number),
                               L(
                                   vidItems.ToArray()
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
