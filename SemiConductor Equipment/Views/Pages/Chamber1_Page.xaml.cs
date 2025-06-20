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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.ViewModels.Pages;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Pages
{
    /// <summary>
    /// Chamber1_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Chamber1_Page : Page
    {
        #region FIELDS
        public Chamber1_ViewModel ViewModel { get; set; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public Chamber1_Page(Chamber1_ViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            this.imgChamber1.Source = new BitmapImage(new Uri("/Resources/Buffer.png", UriKind.Relative));
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LogText")
            {
                tblog.ScrollToEnd();
            }
            else if (e.PropertyName == "IsWafer")
            {
                Change_Image(ViewModel.IsWafer);
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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.OnNavigatedToAsync(1);
        }

        private void Change_Image(bool isChecked)
        {
            if (isChecked)
            {
                imgChamber1.Source = new BitmapImage(new Uri("/Resources/Buffer_in_wafer.png", UriKind.Relative));
            }
            else
            {
                imgChamber1.Source = new BitmapImage(new Uri("/Resources/Buffer.png", UriKind.Relative));
            }
        }

        #endregion
    }
}
