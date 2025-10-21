using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.ViewModels.Windows;
using SemiConductor_Equipment.Views.Pages;
using SemiConductor_Equipment.Views.Windows;

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
        /// <summary>
        /// 알람 메세지 Clear 커맨드
        /// </summary>
        [RelayCommand]
        private void Clear()
        {
            this.AlarmMsg.Clear();
            this._alarm_num = 0;
            this._alarmMsgManager.AlarmMessage_OUT();

        }

        [RelayCommand]
        private void Back()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var mainPage = App.Services.GetRequiredService<MainPage>();
                mainWindow.MainFrame.Navigate(mainPage);
            }
        }

        [RelayCommand]
        private void Open()
        {
            var vm = App.Services.GetRequiredService<AlarmLogHistoryViewModel>();
            var carrierSetupWindow = new AlarmLogHistoryWindow(vm);
            var result = carrierSetupWindow.ShowDialog();
        }
        #endregion

        #region METHOD
        /// <summary>
        /// 알람 메세지 삽입 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
