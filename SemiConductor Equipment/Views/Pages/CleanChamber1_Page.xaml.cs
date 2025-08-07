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

namespace SemiConductor_Equipment.Views.Pages
{
    /// <summary>
    /// Buffer1_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CleanChamber1_Page : Page
    {
        #region FIELDS
        public CleanChamber1_ViewModel ViewModel { get; set; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public CleanChamber1_Page(CleanChamber1_ViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            //this.imgChamber.Source = new BitmapImage(new Uri("/Resources/Clean_Nothing_Wafer_MultiCupDown.png", UriKind.Relative));

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
                this.searchDataLoadingControl.Visibility = Visibility.Visible;
                this.dtgLogViewer.Visibility = Visibility.Collapsed;
                var mainPage = App.Services.GetRequiredService<MainPage>();
                mainWindow.MainFrame.Navigate(mainPage);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Load_Chemical();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Logpagetable": // 이벤트로 온 데이터가 View Model에 ObservableProperty로 선언된 무엇이냐
                    this.searchDataLoadingControl.Visibility = Visibility.Collapsed;
                    this.dtgLogViewer.Visibility = Visibility.Visible;
                    break;

                case "IsWafer":
                    Change_Image(ViewModel.IsWafer);
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
            if (isChecked == 1)
            {
                AnimateMultiCupDown();
            }
            else if (isChecked == 2)
            {
                AnimateMultiCupUp();
            }
            else // 웨이퍼 올라갈때
            {
                //imgChamber.Source = new BitmapImage(new Uri("/Resources/Clean_Nothing_Wafer_MultiCupDown.png", UriKind.Relative));
            }
        }

        private void AnimateMultiCupUp()
        {
            DoubleAnimation moveUpAnimation = new DoubleAnimation
            {
                From = 400,       // 시작 Y 위치
                To = 100,         // 끝 Y 위치 (위로 올라감)
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(moveUpAnimation, ImageMutiCup);
            Storyboard.SetTargetProperty(moveUpAnimation, new PropertyPath("(Canvas.Top)"));

            Storyboard sb = new Storyboard();
            sb.Children.Add(moveUpAnimation);
            sb.Begin();
        }

        private void AnimateMultiCupDown()
        {
            DoubleAnimation moveUpAnimation = new DoubleAnimation
            {
                From = 100,       // 시작 Y 위치
                To = 400,         // 끝 Y 위치 (위로 올라감)
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(moveUpAnimation, ImageMutiCup);
            Storyboard.SetTargetProperty(moveUpAnimation, new PropertyPath("(Canvas.Top)"));

            Storyboard sb = new Storyboard();
            sb.Children.Add(moveUpAnimation);
            sb.Begin();
        }
        #endregion
    }
}
