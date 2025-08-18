using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.ViewModels.Menus;
using SemiConductor_Equipment.Views.Pages;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Menus
{
    public partial class SolutionSettingMenu : Page
    {
        #region FIELDS
        public SolutionMenusViewModel ViewModel { get; set; }
        public Rectangle ClickArea;
        public Rectangle FillLevel;
        public Rectangle PreClean_ClickArea;
        public Rectangle PreClean_FillLevel;

        int maxVolume = 100;
        int currentVolume, currentPreCleanVolume;
        string chamber_name = "Chamber 1";
        #endregion

        #region CONSTRUCTOR
        public SolutionSettingMenu(SolutionMenusViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            ViewModel.FillUpdate += ViewModel_FillUpdate;
            DataContext = this;
        }

        #endregion

        #region METHOD
        void Page_Load(object sender, RoutedEventArgs e)
        {
            //ViewModel.Setup_Config();
            GetVolume();
            UpdateFillLevel();
        }

        private void ViewModel_FillUpdate()
        {
            GetVolume();
            UpdateFillLevel();
        }

        private void GetVolume()
        {
            if (chamber_name == "Chamber 1")
            {
                currentVolume = (int)ViewModel.Chemical_Chamber1;
                currentPreCleanVolume = (int)ViewModel.PreClean_Chamber1;
                ClickArea = ClickArea1;
                PreClean_ClickArea = PreClean_ClickArea1;
                FillLevel = FillLevel1;
                PreClean_FillLevel = PreClean_FillLevel1;
            }
            else if (chamber_name == "Chamber 2")
            {
                currentVolume = (int)ViewModel.Chemical_Chamber2;
                currentPreCleanVolume = (int)ViewModel.PreClean_Chamber2;
                ClickArea = ClickArea2;
                PreClean_ClickArea = PreClean_ClickArea2;
                FillLevel = FillLevel2;
                PreClean_FillLevel = PreClean_FillLevel2;
            }
            else if (chamber_name == "Chamber 3")
            {
                currentVolume = (int)ViewModel.Chemical_Chamber3;
                currentPreCleanVolume = (int)ViewModel.PreClean_Chamber3;
                ClickArea = ClickArea3;
                PreClean_ClickArea = PreClean_ClickArea3;
                FillLevel = FillLevel3;
                PreClean_FillLevel = PreClean_FillLevel3;
            }
            else if (chamber_name == "Chamber 4")
            {
                currentVolume = (int)ViewModel.Chemical_Chamber4;
                currentPreCleanVolume = (int)ViewModel.PreClean_Chamber4;
                ClickArea = ClickArea4;
                PreClean_ClickArea = PreClean_ClickArea4;
                FillLevel = FillLevel4;
                PreClean_FillLevel = PreClean_FillLevel4;
            }
            else if (chamber_name == "Chamber 5")
            {
                currentVolume = (int)ViewModel.Chemical_Chamber5;
                currentPreCleanVolume = (int)ViewModel.PreClean_Chamber5;
                ClickArea = ClickArea5;
                PreClean_ClickArea = PreClean_ClickArea5;
                FillLevel = FillLevel5;
                PreClean_FillLevel = PreClean_FillLevel5;
            }
            else if (chamber_name == "Chamber 6")
            {
                currentVolume = (int)ViewModel.Chemical_Chamber6;
                currentPreCleanVolume = (int)ViewModel.PreClean_Chamber6;
                ClickArea = ClickArea6;
                PreClean_ClickArea = PreClean_ClickArea6;
                FillLevel = FillLevel6;
                PreClean_FillLevel = PreClean_FillLevel6;
            }
        }

        private void SetVolume(int newVolume)
        {
            if (chamber_name == "Chamber 1")
                ViewModel.Chemical_Chamber1 = newVolume;
            else if (chamber_name == "Chamber 2")
                ViewModel.Chemical_Chamber2 = newVolume;
            else if (chamber_name == "Chamber 3")
                ViewModel.Chemical_Chamber3 = newVolume;
            else if (chamber_name == "Chamber 4")
                ViewModel.Chemical_Chamber4 = newVolume;
            else if (chamber_name == "Chamber 5")
                ViewModel.Chemical_Chamber5 = newVolume;
            else if (chamber_name == "Chamber 6")
                ViewModel.Chemical_Chamber6 = newVolume;
        }

        private void UpdateFillLevel()
        {
            int totalHeight = (int)ClickArea.ActualHeight;
            int height = currentVolume * totalHeight / maxVolume; // 정수 연산
            FillLevel.Height = height; // double로 변환되어 들어감

            totalHeight = (int)PreClean_ClickArea.ActualHeight;
            height = currentPreCleanVolume * totalHeight / maxVolume; // 정수 연산
            PreClean_FillLevel.Height = height; // double로 변환되어 들어감
        }

        private void ClickArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetVolumeFromMouse((int)e.GetPosition(ClickArea).Y, ClickArea);
        }

        private void ClickArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetVolumeFromMouse((int)e.GetPosition(ClickArea).Y, ClickArea);
            }
        }

        void SetVolumeFromMouse(int mouseY, Rectangle ClickArea)
        {
            double offsetY = -FillLevel.Margin.Top;
            mouseY -= (int)offsetY;

            int height = (int)ClickArea.ActualHeight;
            int filledHeight = height - mouseY; // 클릭 지점부터 아래까지 채움
            if (filledHeight < 0) filledHeight = 0;
            if (filledHeight > height) filledHeight = height;

            // 비율 → 리터 계산 (정수)
            currentVolume = filledHeight * maxVolume / height;
            SetVolume(currentVolume);
            UpdateFillLevel();
        }

        private void PreClean_SetVolume(int newVolume)
        {
            if (chamber_name == "Chamber 1")
                ViewModel.PreClean_Chamber1 = newVolume;
            else if (chamber_name == "Chamber 2")
                ViewModel.PreClean_Chamber2 = newVolume;
            else if (chamber_name == "Chamber 3")
                ViewModel.PreClean_Chamber3 = newVolume;
            else if (chamber_name == "Chamber 4")
                ViewModel.PreClean_Chamber4 = newVolume;
            else if (chamber_name == "Chamber 5")
                ViewModel.PreClean_Chamber5 = newVolume;
            else if (chamber_name == "Chamber 6")
                ViewModel.PreClean_Chamber6 = newVolume;
        }

        private void PreClean_ClickArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PreClean_SetVolumeFromMouse((int)e.GetPosition(ClickArea).Y, PreClean_ClickArea);
        }

        private void PreClean_ClickArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                PreClean_SetVolumeFromMouse((int)e.GetPosition(ClickArea).Y, PreClean_ClickArea);
            }
        }

        void PreClean_SetVolumeFromMouse(int mouseY, Rectangle PreClean_ClickArea)
        {
            double offsetY = -FillLevel.Margin.Top;
            mouseY -= (int)offsetY;

            int height = (int)PreClean_ClickArea.ActualHeight;
            int filledHeight = height - mouseY; // 클릭 지점부터 아래까지 채움
            if (filledHeight < 0) filledHeight = 0;
            if (filledHeight > height) filledHeight = height;

            // 비율 → 리터 계산 (정수)
            currentPreCleanVolume = filledHeight * maxVolume / height;
            PreClean_SetVolume(currentPreCleanVolume);
            PreClean_UpdateFillLevel();
        }

        private void PreClean_UpdateFillLevel()
        {
            int totalHeight = (int)PreClean_ClickArea.ActualHeight;
            int height = currentPreCleanVolume * totalHeight / maxVolume; // 정수 연산
            PreClean_FillLevel.Height = height; // double로 변환되어 들어감
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

            if (e.Source is TabControl tabControl)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    TabItem selectedTab = tabControl.SelectedItem as TabItem;
                    if (selectedTab != null)
                    {
                        string header = selectedTab.Header as string;
                        chamber_name = header;

                        //ViewModel.Setup_Config();
                        GetVolume();
                        UpdateFillLevel();
                    }
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void Chemical_Changed(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (int.TryParse(textBox.Text, out int value))
            {
                currentVolume = value;
            }
        }

        private void PreClean_ClearClick(object sender, EventArgs e)
        {
            currentPreCleanVolume = 0;
            PreClean_SetVolume(currentPreCleanVolume);
            PreClean_UpdateFillLevel();
        }
        private void PreClean_FillupClick(object sender, EventArgs e)
        {
            currentPreCleanVolume = 100;
            PreClean_SetVolume(currentPreCleanVolume);
            PreClean_UpdateFillLevel();
        }
        private void ClearClick(object sender, EventArgs e)
        {
            currentVolume = 0;
            SetVolume(currentVolume);
            UpdateFillLevel();
        }
        private void FillupClick(object sender, EventArgs e)
        {
            currentVolume = 100;
            SetVolume(currentVolume);
            UpdateFillLevel();
        }
        #endregion
    }
}
