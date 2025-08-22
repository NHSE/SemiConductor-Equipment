using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Models
{
    public partial class ResultData : ObservableObject
    {
        #region FIELDS
        [ObservableProperty]
        private bool _isRunning = false;           // LoadPort 번호
        [ObservableProperty]
        private string _loadPort;           // LoadPort 번호
        [ObservableProperty]
        private int _slotNo;                // 슬롯 번호
        [ObservableProperty]
        private string _carrierID;             // 캐리어 번호
        [ObservableProperty]
        private string _cJID;             // CJ ID
        [ObservableProperty]
        private string _pJID;             // PJ ID
        [ObservableProperty]
        private string _chamberName;        // Dry, Bake, Etch 등
        [ObservableProperty]
        private DateTime _startTime;        // 공정 시작 시간
        [ObservableProperty]
        private DateTime _endTime;          // 공정 종료 시간
        [ObservableProperty]
        private TimeSpan _processDuration;  // 실제 공정 시간
        [ObservableProperty]
        private int _rPM;                // 회전 속도
        [ObservableProperty]
        private int _preClean_Flow;            // 설정된 초당 사용 유량
        [ObservableProperty]
        private int _chemical_Flow;            // 설정된 초당 사용 유량
        [ObservableProperty]
        private int _targetMinTemperature; // 목표 최저 온도
        [ObservableProperty]
        private int _targetMaxTemperature;  // 목표 최고 온도
        [ObservableProperty]
        private int _actualTemperature;  // 실제 온도
        [ObservableProperty]
        private bool _yield;                // 성공여부 (true:Pass, false:Fail)
        [ObservableProperty]
        private bool _hasAlarm;             // 알람 발생여부
        [ObservableProperty]
        private string _errorInfo;          // 에러 상세 정보
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        #endregion

        #region COMMAND
        #endregion

        #region METHOD
        #endregion
    }

    public partial class LoadPortWaferKey : ObservableObject
    {
        [ObservableProperty]
        private int _loadPort;
        [ObservableProperty]
        private int _waferId;

        public LoadPortWaferKey(int loadPort, int waferId)
        {
            _loadPort = loadPort;
            _waferId = waferId;
        }
    }
}
