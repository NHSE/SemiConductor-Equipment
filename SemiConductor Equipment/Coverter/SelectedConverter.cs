using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SemiConductor_Equipment.ViewModels.Windows;

namespace SemiConductor_Equipment.Coverter
{
    public class SelectedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var selectedList = values[0] as ObservableCollection<int>;
            if (selectedList == null || values[1] == null)
                return false;

            int currentSvid = (int)values[1];
            return selectedList.Contains(currentSvid);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var isChecked = (bool)value;
            var currentSvid = System.Convert.ToInt32(parameter);

            var vm = Application.Current.MainWindow.DataContext as CEIDModifyViewModel;
            if (vm == null)
                return null;

            if (isChecked)
            {
                if (!vm.SelectedSvids.Contains(currentSvid))
                    vm.SelectedSvids.Add(currentSvid);
            }
            else
            {
                if (vm.SelectedSvids.Contains(currentSvid))
                    vm.SelectedSvids.Remove(currentSvid);
            }

            return null;
        }
    }
}
