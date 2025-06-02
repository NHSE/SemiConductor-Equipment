using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;
using System.Windows.Threading;
using System.Security.AccessControl;
using System.ComponentModel;
using SemiConductor_Equipment.Models;
using System.Collections;

namespace SemiConductor_Equipment.ViewModels.Pages
{
    public partial class MainPageViewModel : ObservableObject
    {
        #region FIELDS
        private readonly IDateTime _iDateTime;
        private DispatcherTimer _timer;
        private bool _isInitialized = false;
        #endregion

        #region PROPERTIES
        [ObservableProperty]
        private string? _currenttime;
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND

        #endregion

        #region METHOD

        public MainPageViewModel(IDateTime iDateTime)
        {
            _iDateTime = iDateTime ?? throw new ArgumentNullException(nameof(iDateTime));

            // 초기값
            this.Currenttime = _iDateTime.GetCurrentTime()?.ToString();

            // 타이머 설정
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => this.Currenttime = _iDateTime.GetCurrentTime()?.ToString();
            _timer.Start();
        }
        #endregion
    }
}
