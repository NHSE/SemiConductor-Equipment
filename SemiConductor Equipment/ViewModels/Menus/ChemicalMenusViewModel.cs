using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;
using SemiConductor_Equipment.interfaces;
using static SemiConductor_Equipment.Models.EventInfo;

namespace SemiConductor_Equipment.ViewModels.Menus
{
    public partial class ChemicalMenusViewModel : ObservableObject
    {
        #region FIELDS
        private readonly IChemicalManager _configManager;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private double _chamber1;
        [ObservableProperty]
        private double _chamber2;
        [ObservableProperty]
        private double _chamber3;
        [ObservableProperty]
        private double _chamber4;
        [ObservableProperty]
        private double _chamber5;
        [ObservableProperty]
        private double _chamber6;
        #endregion

        #region CONSTRUCTOR
        public ChemicalMenusViewModel(IChemicalManager configManager) 
        { 
            this._configManager = configManager;
            this._configManager.ConfigRead += OnConfigRead;
        }
        #endregion

        #region COMMAND
        [RelayCommand]
        private void Save(string chambername)
        {
            int Value = Get_Value(chambername);
            this._configManager.ModifyChemicalValue(chambername, Value);
        }
        #endregion

        #region METHOD
        public void Setup_Config()
        {
            this._configManager.InitConfig();
        }

        private int Get_Value(string chambername)
        {
            int ret = 0;
            switch (chambername)
            {
                case "Chamber1":
                    ret = (int)this.Chamber1;
                    break;
                case "Chamber2":
                    ret = (int)this.Chamber2;
                    break;
                case "Chamber3":
                    ret = (int)this.Chamber3;
                    break;
                case "Chamber4":
                    ret = (int)this.Chamber4;
                    break;
                case "Chamber5":
                    ret = (int)this.Chamber5;
                    break;
                case "Chamber6":
                    ret = (int)this.Chamber6;
                    break;
            }
            return ret;
        }

        private void OnConfigRead()
        {
            this.Chamber1 = (double)this._configManager.Chemical["Chamber1"];
            this.Chamber2 = (double)this._configManager.Chemical["Chamber2"];
            this.Chamber3 = (double)this._configManager.Chemical["Chamber3"];
            this.Chamber4 = (double)this._configManager.Chemical["Chamber4"];
            this.Chamber5 = (double)this._configManager.Chemical["Chamber5"];
            this.Chamber6 = (double)this._configManager.Chemical["Chamber6"];
        }
        #endregion
    }
}
