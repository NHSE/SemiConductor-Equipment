using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SemiConductor_Equipment.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "SemiConductor Equipment";
    }
}
