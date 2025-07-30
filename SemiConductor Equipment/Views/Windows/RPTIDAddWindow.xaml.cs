using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SemiConductor_Equipment.ViewModels.Windows;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.Views.Windows
{
    /// <summary>
    /// RPTIDAddWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RPTIDAddWindow : Window
    {
        public RPTIDAddViewModel ViewModel { get; }

        public RPTIDAddWindow(RPTIDAddViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;

            ViewModel.CloseRequested += OnClose;
        }

        public void SetItem()
        {
            if (ViewModel is RPTIDAddViewModel vm)
            {
                vm.LoadItem();
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggle && toggle.DataContext is int vid)
            {
                if (ViewModel is RPTIDAddViewModel vm)
                {
                    if (toggle.IsChecked == true && !vm.SelectedSvids.Contains(vid))
                    {
                        vm.SelectedSvids.Add(vid);
                        if (vm.Svid_list != string.Empty)
                            vm.Svid_list += $", {vid}";
                        else
                            vm.Svid_list += $"{vid}";
                    }
                }
            }
        }

        private void ToggleButton_UnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggle && toggle.DataContext is int vid)
            {
                if (ViewModel is RPTIDAddViewModel vm)
                {
                    if (toggle.IsChecked == false && vm.SelectedSvids.Contains(vid))
                    {
                        vm.SelectedSvids.Remove(vid);
                        string target = $"{vid}";

                        vm.Svid_list = string.Join(", ",
                            vm.Svid_list
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Where(s => s != target));
                    }
                }
            }
        }

        private void OnClose()
        {
            this.Close();
        }
    }
}
