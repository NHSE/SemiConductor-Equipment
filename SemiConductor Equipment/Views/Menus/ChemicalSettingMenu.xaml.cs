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
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.ViewModels.Menus;
using SemiConductor_Equipment.Views.Pages;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Menus
{
    /// <summary>
    /// ChemicalSettingMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChemicalSettingMenu : Page
    {

        #region FIELDS
        public ChemicalMenusViewModel ViewModel { get; set; }
        public Rectangle ClickArea;
        public Rectangle FillLevel;
        double maxVolume = 100.0; // 최대 리터
        double currentVolume;
        string chamber_name = "Chamber 1";
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public ChemicalSettingMenu(ChemicalMenusViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        void Page_Load(object sender, RoutedEventArgs e)
        {
            ViewModel.Setup_Config();

            GetVolume();

            UpdateFillLevel();
        }

        private void GetVolume()
        {
            if (chamber_name == "Chamber 1")
            {
                currentVolume = ViewModel.Chamber1;
                ClickArea = ClickArea1;
                FillLevel = FillLevel1;
            }
            else if (chamber_name == "Chamber 2")
            {
                currentVolume = ViewModel.Chamber2;
                ClickArea = ClickArea2;
                FillLevel = FillLevel2;
            }
            else if (chamber_name == "Chamber 3")
            {
                currentVolume = ViewModel.Chamber3;
                ClickArea = ClickArea3;
                FillLevel = FillLevel3;
            }
            else if (chamber_name == "Chamber 4")
            {
                currentVolume = ViewModel.Chamber4;
                ClickArea = ClickArea4;
                FillLevel = FillLevel4;
            }
            else if (chamber_name == "Chamber 5")
            {
                currentVolume = ViewModel.Chamber5;
                ClickArea = ClickArea5;
                FillLevel = FillLevel5;
            }
            else if (chamber_name == "Chamber 6")
            {
                currentVolume = ViewModel.Chamber6;
                ClickArea = ClickArea6;
                FillLevel = FillLevel6;
            }
        }

        private void SetVolume(double newVolume)
        {
            if (chamber_name == "Chamber 1")
                ViewModel.Chamber1 = newVolume;
            else if (chamber_name == "Chamber 2")
                ViewModel.Chamber2 = newVolume;
            else if (chamber_name == "Chamber 3")
                ViewModel.Chamber3 = newVolume;
            else if (chamber_name == "Chamber 4")
                ViewModel.Chamber4 = newVolume;
            else if (chamber_name == "Chamber 5")
                ViewModel.Chamber5 = newVolume;
            else if (chamber_name == "Chamber 6")
                ViewModel.Chamber6 = newVolume;
        }
        private void UpdateFillLevel()
        {
            double percent = currentVolume / maxVolume;
            double totalHeight = ClickArea.ActualHeight;
            FillLevel.Height = percent * totalHeight;
        }

        // 마우스로 클릭한 위치에 따라 chemical 양 조정
        private void ClickArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetVolumeFromMouse(e.GetPosition(ClickArea).Y, ClickArea);
        }

        private void ClickArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetVolumeFromMouse(e.GetPosition(ClickArea).Y, ClickArea);
            }
        }

        void SetVolumeFromMouse(double mouseY, Rectangle ClickArea)
        {
            double height = ClickArea.ActualHeight;
            double percent = (height - mouseY) / height; // 아래서부터 채움
            percent = Math.Clamp(percent, 0, 1);

            currentVolume = Math.Round(percent * maxVolume, 1);
            SetVolume(currentVolume);
            UpdateFillLevel();
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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded) return;
            // 이벤트가 탭컨트롤 Source인지 확인 (종종 다른 컨트롤에서도 이벤트 발생할 수 있음)
            if (e.Source is TabControl tabControl)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    TabItem selectedTab = tabControl.SelectedItem as TabItem;
                    if (selectedTab != null)
                    {
                        string header = selectedTab.Header as string;
                        chamber_name = header;

                        ViewModel.Setup_Config();
                        GetVolume();
                        UpdateFillLevel();
                    }
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void Chemical_Changed(object sender, TextChangedEventArgs e)
        {
            // 사용자가 텍스트 입력할 때마다 호출됨
            var textBox = sender as TextBox;
            currentVolume = Convert.ToDouble(textBox.Text);
        }
        #endregion
    }
}
