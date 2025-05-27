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
using System.Windows.Shapes;
using SemiConductor_Equipment.ViewModels.Windows;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace SemiConductor_Equipment.Views.Windows
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; }
        public MainWindow(MainWindowViewModel viewModel)
        {
            try
            {
                ViewModel = viewModel;
                InitializeComponent();
                DataContext = this;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MainWindow 생성자 예외: " + ex.ToString());
                System.Windows.MessageBox.Show(ex.ToString(), "MainWindow 생성자 예외");
            }
        }
        public void ShowWindow() => Show();

        public void CloseWindow() => Close();

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
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

        private void bMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenContextMenu();
        }

        private void bMenu_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenContextMenu();
            e.Handled = true; // 이벤트가 더 이상 전파되지 않게 막음 (선택)
        }
    }
}
