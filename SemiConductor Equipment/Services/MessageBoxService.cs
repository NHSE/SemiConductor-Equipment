using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Commands;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    public partial class MessageBoxService : IMessageBox
    {
        private readonly Queue<List<string>> _MessageQueue = new();
        public event EventHandler<List<string>> Message_Show;
        private readonly object _lock = new();
        private bool _isProcessing = false;

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
                }
            }
            catch (Exception ex)
            {
                lock (_lock)
                {
                    _isProcessing = false;
                }
                // 로그 남기기 등 추가 처리
            }
        }
    }
}
