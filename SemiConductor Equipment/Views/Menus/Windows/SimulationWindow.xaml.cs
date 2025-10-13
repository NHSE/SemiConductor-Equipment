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
using SemiConductor_Equipment.ViewModels.Menus.Windows;

namespace SemiConductor_Equipment.Views.Menus.Windows
{
    /// <summary>
    /// SimulationWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SimulationWindow : Window
    {
        public SimulationViewModel ViewModel { get; set; }
        public SimulationWindow(SimulationViewModel viewmodel)
        {
            ViewModel = viewmodel;
            DataContext = this;

            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
