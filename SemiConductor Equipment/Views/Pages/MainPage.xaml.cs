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
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.ViewModels.Pages;
using SemiConductor_Equipment.ViewModels.Windows;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Pages
{
    /// <summary>
    /// Page1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainPage : Page
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public MainPageViewModel ViewModel { get; }
        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainPageViewModel(new DateTimeService());
            DataContext = this;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void OpenContextMenu()
        {
            if (!bMenu.ContextMenu.IsOpen)
            {
                bMenu.ContextMenu.PlacementTarget = bMenu;
                bMenu.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                bMenu.ContextMenu.IsOpen = true;
            }
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            OpenContextMenu();
        }

        private void bMenu_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenContextMenu();
            e.Handled = true; // 이벤트가 더 이상 전파되지 않게 막음 (선택)
        }

        private void LoadPort1_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Source = new Uri("../Pages/LoadPort1_Page.xaml", UriKind.Relative);
            }
        }
        private void LoadPort2_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Source = new Uri("../Pages/LoadPort2_Page.xaml", UriKind.Relative);
            }
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Logtables": // 이벤트로 온 데이터가 View Model에 ObservableProperty로 선언된 무엇이냐
                    this.LogLoadingControl.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        #endregion

        private void SubMenu_Log_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Source = new Uri("../Pages/LogPage.xaml", UriKind.Relative);
            }
        }
    }
}
