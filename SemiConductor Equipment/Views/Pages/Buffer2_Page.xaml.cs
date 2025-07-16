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
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.ViewModels.Pages;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Pages
{
    /// <summary>
    /// Buffer2_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Buffer2_Page : Page
    {
        #region FIELDS
        public Buffer_ViewModel ViewModel { get; set; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public Buffer2_Page(Buffer_ViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            { 
                this.searchDataLoadingControl.Visibility = Visibility.Visible;
                this.dtgLogViewer.Visibility = Visibility.Collapsed;
                var mainPage = App.Services.GetRequiredService<MainPage>();
                mainWindow.MainFrame.Navigate(mainPage);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //await ViewModel.OnNavigatedToAsync(2);
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Logpagetable": // 이벤트로 온 데이터가 View Model에 ObservableProperty로 선언된 무엇이냐
                    this.searchDataLoadingControl.Visibility = Visibility.Collapsed;
                    this.dtgLogViewer.Visibility = Visibility.Visible;
                    break;
            }
        }
        #endregion
    }
}
