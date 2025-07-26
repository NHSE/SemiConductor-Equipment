using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;
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
        private CancellationTokenSource _cts;
        private ISecsGem _secs;
        private readonly Queue<CEIDInfo> _EventQue = new();
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public EventMessageService(IEventConfigManager eventConfigManager)
        {
            this._eventConfigManager = eventConfigManager;
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

        public bool IsCEIDEnabled(int ceid)
        {
            return this._eventConfigManager.CEID[ceid].State;
        }

        public void EnqueueEventData(CEIDInfo eventData)
        {
            _EventQue.Enqueue(eventData);
        }

        public void SetSecsGem(ISecsGem secsGem) => _secs = secsGem;

        public async Task ProcessEventQueueAsync(CancellationToken token)
        {
            try
            {
                Console.WriteLine("Start Thread");
                while (!token.IsCancellationRequested)
                {
                    CEIDInfo? eventData = null;

                    if (_EventQue.Count > 0)
                    {
                        eventData = _EventQue.Dequeue();
                        var eventmsg = new SecsMessage(6, 11, false)
                        {
                            Name = "Event Report Send",
                            SecsItem = L(
                               U4(0),
                               U4(1),
                               L(
                                   L(
                                       U4((uint)eventData.Number),
                                       L(
                                            A(eventData.SVIDsDisplay)
                                        )
                                     )
                                 )
                            )
                        };

                        SecsMessage reply = await _secs.SendAsync(eventmsg);
                    }

                    if (eventData == null)
                    {
                        await Task.Delay(100, token);
                        continue;
                    }
                }
            }
            finally
            {
                Console.WriteLine("End Thread");
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
                    // 예외 발생 가능 코
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
                Console.WriteLine("end Thread");
            }
        }
        #endregion

    }
}
