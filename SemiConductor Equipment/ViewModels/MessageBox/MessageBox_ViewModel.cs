using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.ViewModels.MessageBox
{
    public partial class MessageBox_ViewModel : ObservableObject
    {
        #region FIELDS
        private readonly IMessageBox _messageBox;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private string? msg;

        [ObservableProperty]
        private string? title;
        #endregion

        #region CONSTRUCTOR
        public MessageBox_ViewModel(IMessageBox messageBox)
        {
            this._messageBox = messageBox;
            this._messageBox.Message_Show += OnMessageShow;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        private void OnMessageShow(object sender, List<string> args)
        {
            // Dispatcher로 UI 스레드에서 메시지 박스 띄우기
            this.Title = args[0];
            this.Msg = args[1];
        }
        #endregion
    }
}
