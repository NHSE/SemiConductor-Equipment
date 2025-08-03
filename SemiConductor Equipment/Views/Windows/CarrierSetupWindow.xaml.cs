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
using SemiConductor_Equipment.ViewModels.Windows;

namespace SemiConductor_Equipment.Views.Windows // ALL 체크 박스 추가 !!!, 메세지 파싱 !!!, Set 전 Cancel 버튼 비활 -> Set 후 Cancel 버튼 활
{
    /// <summary>
    /// CarrierSetupWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CarrierSetupWindow : Window
    {
        private List<Line> wafers = new List<Line>();
        private const int SlotCount = 25;
        private const double TopY = 90;
        private const double BottomY = 280;
        private const double XStart = 97;
        private const double XEnd = 257;

        public List<int> SelectedWaferSlots { get; private set; } = new();
        private double GapY => (BottomY - TopY) / (SlotCount - 1);

        public CarrierSetupWindow()
        {
            InitializeComponent();
            DataContext = new CarrierSetupViewModel();
            
        }

        private void ImageCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(ImageCanvas);
            int slotIndex = (int)((clickPoint.Y - TopY + GapY / 2) / GapY);
            if (slotIndex < 0 || slotIndex >= SlotCount) return;

            double y = TopY + slotIndex * GapY;
            if (wafers.Any(w => w.Y1 == y)) return;

            var line = new Line
            {
                X1 = XStart,
                X2 = XEnd,
                Y1 = y,
                Y2 = y,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            wafers.Add(line);
            ImageCanvas.Children.Add(line);
            AddWaferToSlot(slotIndex);
        }

        private void ImageCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(ImageCanvas);
            int slotIndex = (int)((clickPoint.Y - TopY + GapY / 2) / GapY);
            if (slotIndex < 0 || slotIndex >= SlotCount) return;

            double y = TopY + slotIndex * GapY;
            var line = wafers.FirstOrDefault(w => w.Y1 == y);
            if (line != null)
            {
                ImageCanvas.Children.Remove(line);
                wafers.Remove(line);
                RemoveWaferFromSlot(slotIndex);
            }
        }

        private void AddWaferToSlot(int slotIndex)
        {
            // 기존 선 추가 코드 외에:
            if (!SelectedWaferSlots.Contains((slotIndex-25) * (-1)))
                SelectedWaferSlots.Add((slotIndex - 25) * (-1));
        }

        private void RemoveWaferFromSlot(int slotIndex)
        {
            if (SelectedWaferSlots.Contains((slotIndex - 25) * (-1)))
                SelectedWaferSlots.Remove((slotIndex - 25) * (-1));
        }

        private void btnset_Click(object sender, RoutedEventArgs e)
        {
            // 창을 닫고 호출자에게 데이터 전달
            this.DialogResult = true;
            this.Close();
        }

        private void DrawAllLines()
        {
            for (int slotIndex = 0; slotIndex < SlotCount; slotIndex++)
            {
                double y = TopY + slotIndex * GapY;
                if (!wafers.Any(w => w.Y1 == y))
                {
                    var line = new Line
                    {
                        X1 = XStart,
                        X2 = XEnd,
                        Y1 = y,
                        Y2 = y,
                        Stroke = Brushes.Red,
                        StrokeThickness = 2
                    };
                    wafers.Add(line);
                    ImageCanvas.Children.Add(line);
                    AddWaferToSlot(slotIndex);
                }
            }
        }

        // 체크박스 체크 시 전체 라인 그리기
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DrawAllLines();
        }

        // 선택: 체크 해제 시 전체 라인 지우기(원하면 사용)
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var line in wafers)
            {
                ImageCanvas.Children.Remove(line);
            }
            wafers.Clear();
        }
    }
}