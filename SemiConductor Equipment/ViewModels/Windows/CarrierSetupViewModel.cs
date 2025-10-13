using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.ViewModels.Windows
{
    public partial class CarrierSetupViewModel : ObservableObject
    {
        #region FIELDS
        #endregion

        #region PROPERTIES

        [ObservableProperty]
        private ObservableCollection<int> _selectedWaferSlots = new();
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        [RelayCommand]
        private void ToggleSlot(int slotIndex)
        {
            if (SelectedWaferSlots.Contains(slotIndex))
                SelectedWaferSlots.Remove(slotIndex);
            else
                SelectedWaferSlots.Add(slotIndex);
        }
        #endregion

        #region METHOD
        #endregion
    }
}
