using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
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
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
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
        public MainPageViewModel ViewModel { get; }
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            this.imgLp1.Source = new BitmapImage(new Uri("/Resources/Carrier_nothing.png", UriKind.Relative));
            this.imgLp2.Source = new BitmapImage(new Uri("/Resources/Carrier_nothing.png", UriKind.Relative));
        }

        #endregion

        #region COMMAND
        #endregion

        #region METHOD

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Secsdatalog")
            {
                this.tbxLogText.ScrollToEnd();
            }
            else if(e.PropertyName == "Loadport1imagepath")
            {
                this.imgLp1.Source = new BitmapImage(new Uri(ViewModel.Loadport1imagepath, UriKind.Relative));
            }
            else if (e.PropertyName == "Loadport2imagepath")
            {
                this.imgLp2.Source = new BitmapImage(new Uri(ViewModel.Loadport2imagepath, UriKind.Relative));
            }
            else if(e.PropertyName == "Equipment_state")
            {
                this.elpsstate.Fill = ViewModel.Equipment_color;
                this.tbkstate.Foreground = ViewModel.Equipment_color;
                this.tbkstate.Text = ViewModel.Equipment_state;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void Scroll()
        {
            if (tbxLogText.Dispatcher.CheckAccess())
            {
                tbxLogText.ScrollToEnd();
            }
            else
            {
                tbxLogText.Dispatcher.Invoke(() =>
                {
                    tbxLogText.ScrollToEnd();
                });
            }
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
                var Change_Page = App.Services.GetRequiredService<LoadPort1_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }
        private void LoadPort2_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var Change_Page = App.Services.GetRequiredService<LoadPort2_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }

        private void SubMenu_Log_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Source = new Uri("../Menus/LogPage.xaml", UriKind.Relative);
            }
        }

        private void Buffer1_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var Change_Page = App.Services.GetRequiredService<Buffer1_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }

        private void Buffer2_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var Change_Page = App.Services.GetRequiredService<Buffer2_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }

        private void Buffer3_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var Change_Page = App.Services.GetRequiredService<Buffer3_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }

        private void Buffer4_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var Change_Page = App.Services.GetRequiredService<Buffer4_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }

        private void Chamber1_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var Change_Page = App.Services.GetRequiredService<Chamber1_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }

        private void Chamber2_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var Change_Page = App.Services.GetRequiredService<Chamber2_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }

        private void Chamber3_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var Change_Page = App.Services.GetRequiredService<Chamber3_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }

        private void Chamber4_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var Change_Page = App.Services.GetRequiredService<Chamber4_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }

        private void Chamber5_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var Change_Page = App.Services.GetRequiredService<Chamber5_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }

        private void Chamber6_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var Change_Page = App.Services.GetRequiredService<Chamber6_Page>();
                mainWindow.MainFrame.Navigate(Change_Page);
            }
        }
        #endregion
    }
}
