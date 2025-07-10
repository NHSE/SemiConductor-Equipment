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
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.ViewModels.Pages;
using SemiConductor_Equipment.Views.Windows;

namespace SemiConductor_Equipment.Views.Pages
{
    /// <summary>
    /// LoadPort2_Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoadPort2_Page : Page
    {
        #region FIELDS
        public LoadPort2_ViewModel ViewModel { get; }
        private Dictionary<int, Line> _slotLines = new();
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        public LoadPort2_Page(LoadPort2_ViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
            viewModel.RemoveRequested += Remove_WaferLine;
            viewModel.AddRequested += Add_WaferLine;
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

        private void btnSetup_Click(object sender, RoutedEventArgs e)
        {
            var carrierSetupWindow = new CarrierSetupWindow();
            var result = carrierSetupWindow.ShowDialog();

            if (result == true)
            {
                var selected = carrierSetupWindow.SelectedWaferSlots;
                // 원하는 방식으로 전달
                ViewModel.SelectedSlots = selected;
                DrawLinesBasedOnSelectedSlots(selected);
                ViewModel.IsSetupEnabled = false;   // Setup 비활성
                ViewModel.IsCancelEnabled = true;   // Cancel 활성
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            mainCanvas.Children.Clear();
        }

        private void Remove_WaferLine(object sender, Wafer wafer)
        {
            if (_slotLines.TryGetValue(wafer.Wafer_Num, out var line))
            {
                mainCanvas.Children.Remove(line);
                _slotLines.Remove(wafer.Wafer_Num);
            }
        }

        private void Add_WaferLine(object sender, Wafer wafer)
        {
            double TopY = 90;
            double BottomY = 350;
            double XStart = 50;
            double XEnd = 270;
            int SlotCount = 25;
            double GapY = (BottomY - TopY) / (SlotCount - 1);

            if (wafer.Wafer_Num < 0 || wafer.Wafer_Num > SlotCount) return;

            double y = BottomY - wafer.Wafer_Num * GapY;

            Brush brush = Brushes.Green;
            if (wafer.Status == "Error")
            {
                brush = Brushes.Red;
            }

            var line = new Line
            {
                X1 = XStart,
                X2 = XEnd,
                Y1 = y,
                Y2 = y,
                Stroke = brush,
                StrokeThickness = 6
            };

            _slotLines[wafer.Wafer_Num] = line;
            mainCanvas.Children.Add(line);
        }

        private void DrawLinesBasedOnSelectedSlots(List<int> selectedSlots)
        {
            mainCanvas.Children.Clear();

            // 선택된 슬롯에 따라 라인 추가
            double TopY = 90;
            double BottomY = 350;
            double XStart = 50;
            double XEnd = 270;
            int SlotCount = 25;
            double GapY = (BottomY - TopY) / (SlotCount - 1);

            foreach (int slotIndex in selectedSlots)
            {
                if (slotIndex < 0 || slotIndex > SlotCount) continue;

                double y = BottomY - slotIndex * GapY;
                var line = new Line
                {
                    X1 = XStart,
                    X2 = XEnd,
                    Y1 = y,
                    Y2 = y,
                    Stroke = Brushes.White,
                    StrokeThickness = 6
                };

                _slotLines[slotIndex] = line;
                mainCanvas.Children.Add(line);
            }
        }
        #endregion
    }
}
