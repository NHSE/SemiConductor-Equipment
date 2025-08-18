using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.ViewModels.Menus
{
    public partial class AlarmLogViewModel : ObservableObject
    {
        #region FIELDS
        private readonly IAlarmMsgManager _alarmMsgManager;
        private int _alarm_num = 0;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private ObservableCollection<AlarmMessage> _alarmMsg = new();
        #endregion

        #region CONSTRUCTOR
        public AlarmLogViewModel(IAlarmMsgManager alarmMsgManager)
        {
            this._alarmMsgManager = alarmMsgManager;
            this._alarmMsgManager.AlarmData += AlarmMsgManager_AlarmData;
        }
        #endregion

        #region COMMAND
        [RelayCommand]
        private void Clear()
        {
            this.AlarmMsg.Clear();
            this._alarm_num = 0;
            this._alarmMsgManager.AlarmMessage_OUT();

        }
        #endregion

        #region METHOD
        private void AlarmMsgManager_AlarmData(object? sender, string e)
        {
            if (e == string.Empty) return;
            if (Application.Current.Dispatcher.CheckAccess())
            {
                this.AlarmMsg.Add(new AlarmMessage
                {
                    AlarmNum = this._alarm_num,
                    AlarmMsg = e,
                    AlarmTime = DateTime.Now,
                }
                );
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.AlarmMsg.Add(new AlarmMessage
                    {
                        AlarmNum = this._alarm_num,
                        AlarmMsg = e,
                        AlarmTime = DateTime.Now,
                    }
                    );
                });
            }

            this._alarm_num++;
        }
        #endregion
    }
}
