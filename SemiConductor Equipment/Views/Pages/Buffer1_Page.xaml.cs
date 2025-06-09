using System.Windows.Controls;
using SemiConductor_Equipment.ViewModels.Pages;
using System.ComponentModel;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Pages
{
    /// <summary>
    /// Buffer1_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Buffer1_Page : Page
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
        public Buffer1_Page()
        {
            InitializeComponent();
            ViewModel = new Buffer_ViewModel(new LogtableService(new LogDatabaseContext()));
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
                mainWindow.MainFrame.Source = new Uri("../Pages/MainPage.xaml", UriKind.Relative);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.OnNavigatedToAsync(1);
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
