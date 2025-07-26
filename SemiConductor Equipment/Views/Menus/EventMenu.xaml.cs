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
using Secs4Net;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.ViewModels.Menus;
using SemiConductor_Equipment.ViewModels.Windows;
using SemiConductor_Equipment.Views.Pages;
using SemiConductor_Equipment.Views.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.Views.Menus
{
    /// <summary>
    /// EventMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EventMenu : Page
    {
        #region FIELDS
        public EventMenusViewModel ViewModel { get; set; }
        private readonly CEIDModifyWindow _ceidWindow;
        private readonly CEIDModifyViewModel _ceidViewModel;
        private readonly IEventConfigManager _configManager;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public EventMenu(EventMenusViewModel viewModel, CEIDModifyWindow ceidWindow, CEIDModifyViewModel ceidViewModel, IEventConfigManager configManager)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
            ViewModel.modify_action += CEID_Modify;

            this._ceidWindow = ceidWindow;
            this._ceidViewModel = ceidViewModel;
            this._configManager = configManager;
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

        private void CEID_Modify()
        {
            //var ceidmodifyWindow = new CEIDModifyWindow();
            //ceidmodifyWindow.ShowDialog();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 마우스가 클릭된 위치에 있는 DataGridRow 확인
            DependencyObject source = (DependencyObject)e.OriginalSource;

            while (source != null && !(source is DataGridRow))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            if (source is DataGridRow row)
            {
                // 클릭된 행의 데이터 아이템 가져오기
                var item = row.Item as CEIDInfo;
                if (item != null)
                {
                    // 여기에 새 창 띄우기 코드 작성
                    var vm = App.Services.GetRequiredService<CEIDModifyViewModel>();
                    var window = new CEIDModifyWindow(vm);
                    window.SetItem(item);
                    window.ShowDialog();
                }
            }
        }
        #endregion
    }
}
