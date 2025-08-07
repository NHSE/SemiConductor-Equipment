using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;
using Wpf.Ui.Abstractions.Controls;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class CleanChamber2_ViewModel : ObservableObject
    {
        #region FIELDS
        private readonly ICleanManager _cleanManager;
        private readonly IChemicalManager _chemicalManager;
        private bool _isInitialized = false;
        private readonly IDatabase<Chamberlogtable>? _database;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private IEnumerable<Chamberlogtable>? _logpagetable;
        [ObservableProperty]
        private int _isWafer = 0;
        [ObservableProperty]
        private int _chemical = 0;

        #endregion

        #region CONSTRUCTOR
        public CleanChamber2_ViewModel(IDatabase<Chamberlogtable> database, ICleanManager cleanManager, IChemicalManager chemicalManager)
        {
            this._database = database;
            this._cleanManager = cleanManager;
            this._chemicalManager = chemicalManager;

            this._cleanManager.DataEnqueued += CleanManager_DataEnqueued;
            this._cleanManager.MultiCupChange += CleanManager_MultiCupChange;
            this._cleanManager.ChemicalChange += CleanManager_ChemicalChange;

            Load_Chemical();
        }
        #endregion

        #region COMMAND
        #endregion

        #region METHOD

        public async Task OnNavigatedToAsync(int? number)
        {
            if (!_isInitialized)
                await InitializeViewModelAsync(number);
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;
        private async Task InitializeViewModelAsync(int? number)
        {
            try
            {
                this.Logpagetable = await Task.Run(() => this._database?.Search($"Buffer{number}"));
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public void Load_Chemical()
        {
            this.Chemical = this._chemicalManager.GetValue("Chamber2");
        }


        private void CleanManager_DataEnqueued(object? sender, CleanChamberStatus cleanChamber)
        {
            if (cleanChamber.ChamberName == "Chamber2")
            {
                this.IsWafer = (this.IsWafer + 1) % 2;
            }
        }

        private void CleanManager_MultiCupChange(object? sender, CleanChamberStatus cleanChamber)
        {
            if (cleanChamber.ChamberName == "Chamber2")
            {
                if (this.IsWafer == 0)
                {
                    this.IsWafer = 1;
                }
                else
                {
                    this.IsWafer = 3 - this.IsWafer;
                }
            }
        }

        private void CleanManager_ChemicalChange(object? sender, ChemicalStatus cleanChamber)
        {
            if (cleanChamber.ChamberName == "Chamber2")
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    if (this._chemicalManager.ConsumeChemical(cleanChamber.ChamberName, cleanChamber.Chemical))
                    {
                        cleanChamber.Result = false;
                        this.Chemical = this._chemicalManager.GetValue(cleanChamber.ChamberName);
                    }
                    else
                    {
                        cleanChamber.Result = true;
                        this.Chemical = this._chemicalManager.GetValue(cleanChamber.ChamberName);

                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (this._chemicalManager.ConsumeChemical(cleanChamber.ChamberName, cleanChamber.Chemical))
                        {
                            cleanChamber.Result = false;
                            this.Chemical = this._chemicalManager.GetValue(cleanChamber.ChamberName);
                        }
                        else
                        {
                            cleanChamber.Result = true;
                            this.Chemical = this._chemicalManager.GetValue(cleanChamber.ChamberName);
                        }
                    });
                }
            }
        }
        #endregion
    }
}
