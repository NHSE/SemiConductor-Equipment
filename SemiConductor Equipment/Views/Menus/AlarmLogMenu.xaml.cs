using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.ViewModels.Menus;
using SemiConductor_Equipment.ViewModels.Windows;
using SemiConductor_Equipment.Views.Pages;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Menus
{
    /// <summary>
    /// ErrorLogMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmLogMenu : Page
    {
        #region FIELDS
        public AlarmLogViewModel ViewModel { get; set; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public AlarmLogMenu(AlarmLogViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var mainPage = App.Services.GetRequiredService<MainPage>();
                mainWindow.MainFrame.Navigate(mainPage);
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e) // command로 빼기
        {
            var vm = App.Services.GetRequiredService<AlarmLogHistoryViewModel>();
            var carrierSetupWindow = new AlarmLogHistoryWindow(vm);
            var result = carrierSetupWindow.ShowDialog();
        }
        #endregion
    }
}
