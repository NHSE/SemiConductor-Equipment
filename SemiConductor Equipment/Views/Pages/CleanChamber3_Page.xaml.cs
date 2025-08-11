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
    /// Buffer3_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CleanChamber3_Page : Page
    {
        #region FIELDS
        public CleanChamber3_ViewModel ViewModel { get; set; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public CleanChamber3_Page(CleanChamber3_ViewModel viewModel)
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
            AnimateMultiCupUp();
            await StartSprayAsync();
            AnimateMultiCupDown();
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

        private void AnimateMultiCupUp()
        {
            DoubleAnimation moveUpAnimation = new DoubleAnimation
            {
                From = 80,       // 시작 Y 위치
                To = 50,         // 끝 Y 위치 (위로 올라감)
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(moveUpAnimation, MutiCup);
            Storyboard.SetTargetProperty(moveUpAnimation, new PropertyPath("(Canvas.Top)"));

            Storyboard sb = new Storyboard();
            sb.Children.Add(moveUpAnimation);
            sb.Begin();
        }

        private void AnimateMultiCupDown()
        {
            DoubleAnimation moveUpAnimation = new DoubleAnimation
            {
                From = 50,       // 시작 Y 위치
                To = 80,         // 끝 Y 위치 (위로 올라감)
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(moveUpAnimation, MutiCup);
            Storyboard.SetTargetProperty(moveUpAnimation, new PropertyPath("(Canvas.Top)"));

            Storyboard sb = new Storyboard();
            sb.Children.Add(moveUpAnimation);
            sb.Begin();
        }

        private Random _rand = new Random();

        private async Task StartSprayAsync()
        {
            Nozzle.Visibility = Visibility.Visible;

            for (int i = 0; i < 50; i++) // 50개의 물방울 생성
            {
                SprayWater();
                await Task.Delay(50); // 0.05초 간격
            }

            Nozzle.Visibility = Visibility.Collapsed;
        }

        private void SprayWater()
        {
            Ellipse drop = new Ellipse
            {
                Width = 4,
                Height = 4,
                Fill = Brushes.LightSkyBlue,
                Opacity = 0.8
            };

            // 시작 위치 (SprayCanvas 좌표 기준)
            Canvas.SetLeft(drop, 150);
            Canvas.SetTop(drop, 0);

            SprayCanvas.Children.Add(drop);

            // Y 방향 애니메이션
            var dropAnimY = new DoubleAnimation(0, 150, TimeSpan.FromSeconds(0.5))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            dropAnimY.Completed += (s, e) => SprayCanvas.Children.Remove(drop);
            drop.BeginAnimation(Canvas.TopProperty, dropAnimY);

            // X 방향 랜덤 애니메이션
            var dropAnimX = new DoubleAnimation(150, 100 + _rand.NextDouble() * 100, TimeSpan.FromSeconds(0.5));
            drop.BeginAnimation(Canvas.LeftProperty, dropAnimX);
        }
        #endregion
    }
}
