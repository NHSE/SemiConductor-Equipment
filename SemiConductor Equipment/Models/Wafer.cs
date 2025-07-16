using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Models
{
    public partial class Wafer : ObservableObject
    {
        [ObservableProperty]
        private int loadportId;

        [ObservableProperty]
        private int wafer_Num;

        [ObservableProperty]
        private string slotId = "";

        [ObservableProperty]
        private string carrierId = "";

        [ObservableProperty]
        private string lotId = "";

        [ObservableProperty]
        private string pJId = "";

        [ObservableProperty]
        private string cJId = "";

        [ObservableProperty]
        public string currentLocation = "";

        [ObservableProperty]
        public string targetLocation = "";

        [ObservableProperty]
        public bool processing = false;

        [ObservableProperty]
        public string? status = "Ready";

        [ObservableProperty]
        private double positionX;

        [ObservableProperty]
        private double positionY;

        [ObservableProperty]
        private bool isVisible;

        [ObservableProperty]
        private double requiredTemperature;

        [ObservableProperty]
        private double runningTime;
    }
}
