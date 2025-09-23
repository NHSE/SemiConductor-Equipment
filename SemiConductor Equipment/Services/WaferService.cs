using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.Services
{
    public class WaferService : IWaferManager
    {
        #region FIELDS
        private Queue<Wafer> _waferQueue = new();
        public event Action<Wafer> WaferEnqueued;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        /// <summary>
        /// 웨이퍼 데이터를 공정 진행 큐에 삽입하는 메서드
        /// </summary>
        /// <param name="wafer"></param>
        public void Enqueue(Wafer wafer)
        {
            _waferQueue.Enqueue(wafer);
        }

        /// <summary>
        /// 공정 진행 큐를 Dequeue
        /// </summary>
        /// <returns></returns>
        public Wafer? Dequeue()
        {
            return _waferQueue.Count > 0 ? _waferQueue.Dequeue() : null;
        }

        /// <summary>
        /// 큐를 가져오는 메서드
        /// </summary>
        /// <returns>공정 진행 예정 큐</returns>
        public Queue<Wafer> GetQueue()
        {
            return _waferQueue;
        }
        
        /// <summary>
        /// 공정 진행 큐를 비우는 메서드
        /// </summary>
        public void Clear()
        {
            _waferQueue.Clear();
        }
        #endregion
    }
}
