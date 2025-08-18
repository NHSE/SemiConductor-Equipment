using System.Windows.Controls;
using SemiConductor_Equipment.ViewModels.Pages;
using System.ComponentModel;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SemiConductor_Equipment.Views.Pages
{
    /// <summary>
    /// Buffer4_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CleanChamber6_Page : Page
    {
        #region FIELDS
        public CleanChamber6_ViewModel ViewModel { get; set; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public CleanChamber6_Page(CleanChamber6_ViewModel viewModel)
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

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var mainPage = App.Services.GetRequiredService<MainPage>();
                mainWindow.MainFrame.Navigate(mainPage);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Load_Chemical();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsWafer":
                    Change_Image(ViewModel.IsWafer);
                    break;
                case "LogText":
                    tblog.ScrollToEnd();
                    break;
            }
        }

        private void Change_Image(int isChecked)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                Setting_Image(isChecked);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Setting_Image(isChecked);
                });
            }
        }

        private void Setting_Image(int isChecked)
        {
            if (isChecked == 1) // 멀티컵이 올라갈때
            {
                //AnimateMultiCupUpSequence();
            }
            else if (isChecked == 2) // 멀티컵이 내려갈때
            {
                //AnimateMultiCupDownSequence();
            }
            else // 웨이퍼 올라갈때
            {
                //StartScenario();
            }
        }

        
        #endregion
    }
}
