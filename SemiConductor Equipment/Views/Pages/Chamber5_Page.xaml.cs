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
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.ViewModels.Pages;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Pages
{
    /// <summary>
    /// Chamber5_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Chamber5_Page : Page
    {
        #region FIELDS
        public Chamber_ViewModel ViewModel { get; set; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public Chamber5_Page()
        {
            InitializeComponent();
            ViewModel = new Chamber_ViewModel(new ChamberStatusService(new LogDatabaseContext()), new LogService(@"C:\Logs"), new ChamberService(), new MessageBoxService(), 5);
            DataContext = this;

            ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.LogText))
                {
                    tblog.ScrollToEnd();
                }
            };
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
                mainWindow.MainFrame.Source = new Uri("../Pages/MainPage.xaml", UriKind.Relative);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.OnNavigatedToAsync(5);
        }
        #endregion
    }
}
