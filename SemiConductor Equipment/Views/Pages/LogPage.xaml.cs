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
using SemiConductor_Equipment.ViewModels.Pages;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Storage;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Pages
{
    /// <summary>
    /// LogPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LogPage : Page
    {
        public LogPageViewModel ViewModel { get; set; }
        public LogPage()
        {
            InitializeComponent();
            ViewModel = new LogPageViewModel(new LogtableService(new LogDatabaseContext()));
            DataContext = this;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.OnNavigatedToAsync();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Source = new Uri("../Pages/MainPage.xaml", UriKind.Relative);
            }
        }
    }
}
