using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.ViewModels.Menus;
using SemiConductor_Equipment.Views.Pages;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Menus
{
    /// <summary>
    /// EquipMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EquipMenu : Page
    {

        #region FIELDS
        private readonly IMessageBox _messageBox;
        public EquipMenusViewModel ViewModel { get; set; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public EquipMenu(IMessageBox messageBox, EquipMenusViewModel viewModel)
        {
            InitializeComponent();
            this._messageBox = messageBox;
            ViewModel = viewModel;
            DataContext = this;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        private void VerPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 숫자만 허용
            Regex regex = new Regex("[^0-9]+");
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = (TextBox)sender;
            textBox.MaxLength = 3;

            this.tbAllowable.MaxLength = 1;

            // 현재 텍스트에 입력값이 추가된 결과 예측
            string newText;
            if (textBox.SelectionLength > 0)
            {
                // 일부 선택된 상태에서 입력되는 경우
                newText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                          .Insert(textBox.SelectionStart, e.Text);
            }
            else
            {
                // 일반 입력
                newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            }

            // 빈 값이 아니고, 200 이상인지 확인
            if (int.TryParse(newText, out int value))
            {
                if (value > 200)
                {
                    //MessageBox.Show("200 이하만 입력할 수 있습니다.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this._messageBox.Show("입력 초과", "200 이하만 입력할 수 있습니다.");
                    e.Handled = true; // 200 초과 입력 막기
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var mainPage = App.Services.GetRequiredService<MainPage>();
                mainWindow.MainFrame.Navigate(mainPage);
            }
        }
        #endregion
    }
}
