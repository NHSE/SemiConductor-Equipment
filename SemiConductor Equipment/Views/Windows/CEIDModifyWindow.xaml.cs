using SemiConductor_Equipment.ViewModels.Pages;
using SemiConductor_Equipment.ViewModels.Windows;
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
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.Views.Windows
{
    /// <summary>
    /// CEIDModifyWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CEIDModifyWindow : Window
    {

        public CEIDModifyViewModel ViewModel { get; }

        public CEIDModifyWindow(CEIDModifyViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;

            ViewModel.CloseRequested += OnClose;
            ViewModel.PropertyChanged += OnPropertyChanged;
        }

        private void Initialize(bool State)
        {
            if (!State)
            {
                this.tgbstate.Content = "OFF";
            }
            else
            {
                this.tgbstate.Content = "ON";
            }
        }

        public void SetItem(CEIDInfo item)
        {
            if (ViewModel is CEIDModifyViewModel vm)
            {
                vm.LoadItem(item);

                Initialize(item.State);
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggle && toggle.DataContext is int vid)
            {
                if (ViewModel is CEIDModifyViewModel vm)
                {
                    if (toggle.IsChecked == true && !vm.SelectedSvids.Contains(vid))
                    {
                        vm.SelectedSvids.Add(vid);
                        if(vm.Svid_list != string.Empty)
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
                if (ViewModel is CEIDModifyViewModel vm)
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

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "State")
            {
                if (this.tgbstate.Content == "ON")
                {
                    this.tgbstate.Content = "OFF";
                }
                else
                {
                    this.tgbstate.Content = "ON";
                }
            }
        }

        private void OnClose()
        {
            this.Close();
        }
    }
}
