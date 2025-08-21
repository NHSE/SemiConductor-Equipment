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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.ViewModels.Pages;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Pages
{
    /// <summary>
    /// Chamber2_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Chamber2_Page : Page
    {
        #region FIELDS
        public Chamber2_ViewModel ViewModel { get; set; }
        private Random rand = new Random();
        private bool test = false;
        private double _currentAngle = 0;
        private double _currentRpm = 60;
        private DateTime _lastFrameTime;
        private bool _spinning = false;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public Chamber2_Page(Chamber2_ViewModel viewModel)
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
            else if(e.PropertyName == "WaferRPM")
            {
                if (!test)
                {
                    StartSpin();
                    SetRpm(ViewModel.WaferRPM);
                }
                else
                    SetRpm(ViewModel.WaferRPM);

            }
            else if (e.PropertyName == "WaferColor")
            {
                
            }
        }

        public void SetRpm(double rpm)
        {
            _currentRpm = Math.Max(1, rpm);
        }

        private void StartSpin()
        {
            if (_spinning) return;
            _spinning = true;
            _lastFrameTime = DateTime.Now;
            CompositionTarget.Rendering += MotorTick;
        }

        private void MotorTick(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            double deltaTime = (now - _lastFrameTime).TotalSeconds;
            _lastFrameTime = now;

            if (_currentRpm <= 0)
            {
                _spinning = false;
                CompositionTarget.Rendering -= MotorTick;
                return;
            }

            // deltaAngle = 초당 회전 각 * 경과 시간
            double degreesPerSecond = _currentRpm * 360.0 / 60.0;
            _currentAngle += degreesPerSecond * deltaTime;
            _currentAngle %= 360;

            WaferRotate.Angle = _currentAngle;
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

        private void Change_Image(bool isChecked)
        {
            if (isChecked)
            {
                WaferGroup.Visibility = Visibility.Visible;
            }
            else
            {
                WaferGroup.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
    }
}
