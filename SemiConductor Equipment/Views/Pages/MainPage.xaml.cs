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
        private readonly Dictionary<string, Point> locationPositions = new();
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
            locationPositions["Buffer1"] = btnBuffer1.TranslatePoint(new Point(0, 0), Animationctr);
            locationPositions["Buffer2"] = btnBuffer2.TranslatePoint(new Point(0, 0), Animationctr);
            locationPositions["Buffer3"] = btnBuffer3.TranslatePoint(new Point(0, 0), Animationctr);
            locationPositions["Buffer4"] = btnBuffer4.TranslatePoint(new Point(0, 0), Animationctr);

            locationPositions["Chamber1"] = btnChamber1.TranslatePoint(new Point(0, 0), Animationctr);
            locationPositions["Chamber2"] = btnChamber2.TranslatePoint(new Point(0, 0), Animationctr);
            locationPositions["Chamber3"] = btnChamber3.TranslatePoint(new Point(0, 0), Animationctr);
            locationPositions["Chamber4"] = btnChamber4.TranslatePoint(new Point(0, 0), Animationctr);
            locationPositions["Chamber5"] = btnChamber5.TranslatePoint(new Point(0, 0), Animationctr);
            locationPositions["Chamber6"] = btnChamber6.TranslatePoint(new Point(0, 0), Animationctr);

            locationPositions["RobotArm"] = btnRobotArm.TranslatePoint(new Point(0, 0), Animationctr);

            locationPositions["LoadPort1"] = new Point(0, 0);
            locationPositions["LoadPort2"] = new Point(0, 0);

            ViewModel.locationPositions = locationPositions;
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
        #endregion
    }
}
