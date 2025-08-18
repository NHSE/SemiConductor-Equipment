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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;
using SemiConductor_Equipment.ViewModels.Menus;
using SemiConductor_Equipment.ViewModels.Windows;

namespace SemiConductor_Equipment.Views.Windows
{
    /// <summary>
    /// AlarmLogHistoryWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmLogHistoryWindow : Window
    {
        public AlarmLogHistoryViewModel ViewModel { get; set; }
        public AlarmLogHistoryWindow(AlarmLogHistoryViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.OnNavigatedToAsync();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Logpagetable": // 이벤트로 온 데이터가 View Model에 ObservableProperty로 선언된 무엇이냐
                    this.searchDataLoadingControl.Visibility = Visibility.Collapsed;
                    this.dtgLogViewer.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
