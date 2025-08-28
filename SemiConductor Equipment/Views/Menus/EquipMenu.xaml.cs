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

            this.tbCleanrpm.MaxLength = 4;
            this.tbchemical.MaxLength = 2;
            this.tbSpraytime.MaxLength = 2;
            this.tbDryrpm.MaxLength = 4;

            //RPM, Spray, flow rate 최대 길이 정하기

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
             
            if (int.TryParse(newText, out int value))
            {
                switch (textBox.Name)
                {
                    case "tbchemical":
                        if (value > 10)
                        {
                            this._messageBox.Show("입력 초과", "해당 입력은 10 이하만 가능합니다.");
                            e.Handled = true;
                        }
                        break;

                    case "tbSpraytime":
                        if (value > 60)
                        {
                            this._messageBox.Show("입력 초과", "해당 입력은 60 이하만 가능합니다.");
                            e.Handled = true;
                        }
                        break;

                    case "tbDryrpm":
                        if (value > 3000 || value < 10)
                        {
                            this._messageBox.Show("입력 초과", "해당 입력은 10 이상 3000이하만 가능합니다.");
                            e.Handled = true;
                        }
                            break;

                    case "tbCleanrpm":
                        if (value > 3000 || value < 10)
                        {
                            this._messageBox.Show("입력 초과", "해당 입력은 10 이상 3000이하만 가능합니다.");
                            e.Handled = true;
                        }
                        break;

                    case "tbpreclean":
                        if (value > 10)
                        {
                            this._messageBox.Show("입력 초과", "해당 입력은 10 이하만 가능합니다.");
                            e.Handled = true;
                        }
                        break;

                    case "tbpreCleanSpraytime":
                        if (value > 60)
                        {
                            this._messageBox.Show("입력 초과", "해당 입력은 60 이하만 가능합니다.");
                            e.Handled = true;
                        }
                        break;

                    default:
                        if (value > 200)
                        {
                            this._messageBox.Show("입력 초과", "해당 입력은 200 이하만 입력할 수 있습니다.");
                            e.Handled = true; // 200 초과 입력 막기
                        }
                        break;
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
