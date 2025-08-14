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
using Microsoft.Extensions.DependencyInjection;
using SemiConductor_Equipment.ViewModels.Windows;
using SemiConductor_Equipment.Views.Menus;
using SemiConductor_Equipment.Views.MessageBox;
using SemiConductor_Equipment.Views.Pages;
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

        #region FIELDS
        public MainWindowViewModel ViewModel { get; }
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public MainWindow(MainWindowViewModel viewModel)
        {
            try
            {
                ViewModel = viewModel;
                DataContext = this;
                InitializeComponent();

                // 앱 실행 시 인스턴스 생성
                App.Services.GetRequiredService<MessageBoxWindow>();
                App.Services.GetRequiredService<Chamber1_Page>();
                App.Services.GetRequiredService<Chamber2_Page>();
                App.Services.GetRequiredService<Chamber3_Page>();
                App.Services.GetRequiredService<Chamber4_Page>();
                App.Services.GetRequiredService<Chamber5_Page>();
                App.Services.GetRequiredService<Chamber6_Page>();
                App.Services.GetRequiredService<CleanChamber1_Page>();
                App.Services.GetRequiredService<CleanChamber2_Page>();
                App.Services.GetRequiredService<CleanChamber3_Page>();
                App.Services.GetRequiredService<CleanChamber4_Page>();
                //App.Services.GetRequiredService<CleanChamber5_Page>();
                //App.Services.GetRequiredService<CleanChamber6_Page>();
                //

                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    var mainPage = App.Services.GetRequiredService<MainPage>();
                    mainWindow.MainFrame.Navigate(mainPage);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MainWindow 생성자 예외: " + ex.ToString());
                System.Windows.MessageBox.Show(ex.ToString(), "MainWindow 생성자 예외");
            }
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
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
        #endregion

    }
}
