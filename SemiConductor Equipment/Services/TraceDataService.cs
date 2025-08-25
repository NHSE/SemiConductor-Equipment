using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Secs4Net;
using static Secs4Net.Item;
using SemiConductor_Equipment.interfaces;
using Wpf.Ui.Controls;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Secs4Net.Sml;

namespace SemiConductor_Equipment.Services
{
    public class TraceDataService : ITraceDataManager
    {
        #region FIELDS
        private const int MAX_TASK = 4;

        private ISecsGem _secs;
        private ISecsConnection _connection;
        private readonly ILogManager _logManager;
        private readonly IVIDManager _vIDManager;

        private CancellationTokenSource[] ctsList = new CancellationTokenSource[MAX_TASK];
        private Task[] tasks = new Task[MAX_TASK];
        Dictionary<string, int> Task_Data = new Dictionary<string, int>();
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public TraceDataService(IVIDManager vIDManager, ILogManager logManager)
        {
            this._vIDManager = vIDManager;
            this._logManager = logManager;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD

        public void SetSecsGem(ISecsGem secsGem) => _secs = secsGem;

        public void SetConnect(ISecsConnection connection)
        {
            _connection = connection;
        }

        public bool SetTraceData(string TRID, string DSPER, uint TOTSMP, uint REPGSZ, List<uint> VID)
        {
            foreach (var vid in VID)
            {
                if(!this._vIDManager.IsVID((uint)vid))
                {
                    return false;  // <InterLock> 존재하지 않은 VID 설정 시 거부
                }
            }

            if (Task_Data != null && Task_Data.ContainsKey(TRID))
            {
                if (TOTSMP != 0) return false; // <InterLock> Trace Data 종료가 아니면 모두 다 거부

                int id = Task_Data[TRID];
                CancelTask(id);
                RemoveTask(id);
            }
            else
            {
                if(TRID.Count() == 4)
                {
                    return false;
                }
                else
                {
                    int id = -1;

                    // null 자리 찾기
                    for (int i = 0; i < MAX_TASK; i++)
                    {
                        if (ctsList[i] == null)
                        {
                            id = i;
                            break;
                        }
                    }

                    Task_Data[TRID] = id;
                    ctsList[id] = new CancellationTokenSource();
                    tasks[id] = RunTask(TRID, DSPER, TOTSMP, REPGSZ, VID, ctsList[id].Token);
                }
            }
            return true;
        }

        private async Task RunTask(string TRID, string DSPER, uint TOTSMP, uint REPGSZ, List<uint> VID, CancellationToken token)
        {
            try
            { 
                var vidItems = new List<Item>();
                var Items = new List<Item>();

                int hours = int.Parse(DSPER.Substring(0, 2));
                int minutes = int.Parse(DSPER.Substring(2, 2));
                int seconds = int.Parse(DSPER.Substring(4, 2));
                TimeSpan delayTime = new TimeSpan(hours, minutes, seconds);

                int SMPLN = 1;

                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(delayTime);

                    foreach (var v in VID)
                    {
                        //var vid = this._vIDManager.GetSVID(v);
                        vidItems.Add(
                                        A(v.ToString() ?? "")
                                    );
                    }

                    Items.Add(
                                L(
                                    A(TRID),
                                    U4((uint)SMPLN),
                                    A(DateTime.Now.ToString("yyMMddhhmmss")),
                                    L(
                                        vidItems
                                    )
                                )
                            );

                    if (vidItems.Count / VID.Count == REPGSZ)
                    {
                        Item Itemmsg = L(Items);
                        var tracemsg = new SecsMessage(6, 1, false)
                        {
                            Name = "Trace Data",
                            SecsItem = Itemmsg
                        };

                        await _secs.SendAsync(tracemsg);
                        string send_logMessage = $"[SEND] → S6F1\n{tracemsg.ToSml()}";
                        _logManager.WriteLog("Event", "SEND", send_logMessage);

                        vidItems.Clear();
                    }

                    if (SMPLN == TOTSMP)
                    {
                        Task_Data.Remove(TRID);
                        break;
                    }

                    SMPLN++;
                }
            }
            finally
            {
                int id = Task_Data[TRID];
                RemoveTask(id);
            }
        }

        private void CancelTask(int id)
        {
            ctsList[id]?.Cancel();
        }

        private void RemoveTask(int id)
        {
            ctsList[id]?.Dispose();
            ctsList[id] = null;

            string keyToRemove = null;
            foreach (var kvp in Task_Data)
            {
                if (kvp.Value == id)
                {
                    keyToRemove = kvp.Key;
                    break;
                }
            }
            if (keyToRemove != null)
                Task_Data.Remove(keyToRemove);

            tasks[id] = null;
        }
        #endregion
    }
}
