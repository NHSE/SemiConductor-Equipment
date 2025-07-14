using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.ViewModels.MessageBox;

namespace SemiConductor_Equipment.Views.MessageBox
{
    /// <summary>
    /// MessageBoxWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        #region FIELDS
        public MessageBox_ViewModel ViewModel { get; set; }
        #endregion

        #region PROPERTIES

        #endregion

        #region CONSTRUCTOR
        public MessageBoxWindow(MessageBox_ViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.Msg) && !string.IsNullOrEmpty(ViewModel.Msg))
            {
                Dispatcher.Invoke(() =>
                {
                    var win = new MessageBoxWindow(ViewModel);
                    win.ShowDialog();
                    // Msg 초기화로 중복 방지
                    ViewModel.Msg = null;
                });
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        #endregion
    }
}
