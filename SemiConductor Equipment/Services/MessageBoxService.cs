using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    public class MessageBoxService : IMessageBox
    {
        #region FIELDS
        private readonly ILogManager _logManager;
        private readonly Queue<List<string>> _MessageQueue = new();
        public event EventHandler<List<string>> Message_Show;
        private readonly object _lock = new();
        private bool _isProcessing = false;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// 커스텀 메세지 박스를 담당하는 서비스 레이어
        /// </summary>
        /// <param name="logManager"></param>
        public MessageBoxService(ILogManager logManager)
        {
            _logManager = logManager;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        /// <summary>
        /// 메세지 큐 내 데이터 삽입 메서드
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public void Show(string title, string message)
        {
            List<string> list = new List<string>();
            list.Add(title);
            list.Add(message);

            lock (_lock)
            {
                _MessageQueue.Enqueue(list);
                if (!_isProcessing)
                {
                    _isProcessing = true;
                    ThreadPool.QueueUserWorkItem(_ => ProcessQueue());
                }
            }
        }
        
        /// <summary>
        /// 메세지 큐 내 데이터 확인 후 메세지 창으로 보여주는 메서드
        /// </summary>
        private void ProcessQueue()
        {
            try
            {
                while (true)
                {
                    List<string> item;
                    lock (_lock)
                    {
                        if (_MessageQueue.Count == 0)
                        {
                            _isProcessing = false;
                            break;
                        }
                        item = _MessageQueue.Dequeue();
                    }
                    Message_Show?.Invoke(this, item);
                    this._logManager.WriteLog("Error", "SYSTEM", item[1]);
                }
            }
            catch (Exception ex)
            {
                lock (_lock)
                {
                    _isProcessing = false;
                }

                this._logManager.WriteLog("Error", "SYSTEM", "Message Box를 정상적으로 불러오지 못했습니다.");
            }
        }
        #endregion


    }
}
