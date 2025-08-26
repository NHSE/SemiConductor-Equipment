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
    public partial class SolutionMenusViewModel : ObservableObject
    {
        #region FIELDS
        private readonly ISolutionManager _configManager;
        private readonly IVIDManager _vIDManager;
        public event Action FillUpdate;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private double _chemical_Chamber1;
        [ObservableProperty]
        private double _chemical_Chamber2;
        [ObservableProperty]
        private double _chemical_Chamber3;
        [ObservableProperty]
        private double _chemical_Chamber4;
        [ObservableProperty]
        private double _chemical_Chamber5;
        [ObservableProperty]
        private double _chemical_Chamber6;
        [ObservableProperty]
        private double _preClean_Chamber1;
        [ObservableProperty]
        private double _preClean_Chamber2;
        [ObservableProperty]
        private double _preClean_Chamber3;
        [ObservableProperty]
        private double _preClean_Chamber4;
        [ObservableProperty]
        private double _preClean_Chamber5;
        [ObservableProperty]
        private double _preClean_Chamber6;
        #endregion

        #region CONSTRUCTOR
        public SolutionMenusViewModel(ISolutionManager configManager, IVIDManager vIDManager) 
        { 
            this._configManager = configManager;
            this._vIDManager = vIDManager;
            this._configManager.ConfigRead += OnConfigRead;

            Setup_Config();
        }
        #endregion

        #region COMMAND
        [RelayCommand]
        private void Save(string chambername)
        {
            int Value = Get_Value(chambername);
            this._configManager.ModifyChemicalValue(chambername, Value);
        }

        [RelayCommand]
        private void PreClean_Save(string chambername)
        {
            int Value = Get_PreClean_Value(chambername);
            this._configManager.ModifyPreCleanValue(chambername, Value);
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
                    ret = (int)this.Chemical_Chamber1;
                    break;
                case "Chamber2":
                    ret = (int)this.Chemical_Chamber2;
                    break;
                case "Chamber3":
                    ret = (int)this.Chemical_Chamber3;
                    break;
                case "Chamber4":
                    ret = (int)this.Chemical_Chamber4;
                    break;
                case "Chamber5":
                    ret = (int)this.Chemical_Chamber5;
                    break;
                case "Chamber6":
                    ret = (int)this.Chemical_Chamber6;
                    break;
            }
            return ret;
        }

        private int Get_PreClean_Value(string chambername)
        {
            int ret = 0;
            switch (chambername)
            {
                case "Chamber1":
                    ret = (int)this.PreClean_Chamber1;
                    break;
                case "Chamber2":
                    ret = (int)this.PreClean_Chamber2;
                    break;
                case "Chamber3":
                    ret = (int)this.PreClean_Chamber3;
                    break;
                case "Chamber4":
                    ret = (int)this.PreClean_Chamber4;
                    break;
                case "Chamber5":
                    ret = (int)this.PreClean_Chamber5;
                    break;
                case "Chamber6":
                    ret = (int)this.PreClean_Chamber6;
                    break;
            }
            return ret;
        }

        private void SetVID()
        {
            this._vIDManager.SetSVID(104, this.Chemical_Chamber1);
            this._vIDManager.SetSVID(105, this.Chemical_Chamber2);
            this._vIDManager.SetSVID(106, this.Chemical_Chamber3);
            this._vIDManager.SetSVID(107, this.Chemical_Chamber4);
            this._vIDManager.SetSVID(108, this.Chemical_Chamber5);
            this._vIDManager.SetSVID(109, this.Chemical_Chamber6);

            this._vIDManager.SetSVID(110, this.PreClean_Chamber1);
            this._vIDManager.SetSVID(111, this.PreClean_Chamber2);
            this._vIDManager.SetSVID(112, this.PreClean_Chamber3);
            this._vIDManager.SetSVID(113, this.PreClean_Chamber4);
            this._vIDManager.SetSVID(114, this.PreClean_Chamber5);
            this._vIDManager.SetSVID(115, this.PreClean_Chamber6);
        }

        private void OnConfigRead()
        {
            this.Chemical_Chamber1 = (double)this._configManager.Chemical["Chamber1"];
            this.Chemical_Chamber2 = (double)this._configManager.Chemical["Chamber2"];
            this.Chemical_Chamber3 = (double)this._configManager.Chemical["Chamber3"];
            this.Chemical_Chamber4 = (double)this._configManager.Chemical["Chamber4"];
            this.Chemical_Chamber5 = (double)this._configManager.Chemical["Chamber5"];
            this.Chemical_Chamber6 = (double)this._configManager.Chemical["Chamber6"];

            this.PreClean_Chamber1 = (double)this._configManager.PreClean["Chamber1"];
            this.PreClean_Chamber2 = (double)this._configManager.PreClean["Chamber2"];
            this.PreClean_Chamber3 = (double)this._configManager.PreClean["Chamber3"];
            this.PreClean_Chamber4 = (double)this._configManager.PreClean["Chamber4"];
            this.PreClean_Chamber5 = (double)this._configManager.PreClean["Chamber5"];
            this.PreClean_Chamber6 = (double)this._configManager.PreClean["Chamber6"];

            SetVID();
            FillUpdate?.Invoke();
        }
        #endregion
    }
}
