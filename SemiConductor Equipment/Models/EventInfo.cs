using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenTK.Graphics.OpenGL.GL;

namespace SemiConductor_Equipment.Models
{
    public partial class EventInfo
    {
        public partial class RPTIDInfo : ObservableObject
        {
            [ObservableProperty]
            public int number;
            [ObservableProperty]
            public List<int> number_list = new();

            private List<int> vIDs;
            public List<int> VIDs
            {
                get => vIDs;
                set
                {
                    if (SetProperty(ref vIDs, value))
                    {
                        OnPropertyChanged(nameof(VIDsDisplay));
                    }
                }
            }

            public string? VIDsDisplay
            {
                get => vIDs != null ? string.Join(", ", vIDs) : string.Empty;
                set
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        vIDs = value
                            .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => int.TryParse(s, out int n) ? n : -1)
                            .Where(n => n >= 0)
                            .ToList();
                        OnPropertyChanged(nameof(VIDsDisplay));
                    }
                }
            }
        }

        public partial class CEIDInfo : ObservableObject
        {
            [ObservableProperty]
            private int number;

            [ObservableProperty]
            private string name;

            [ObservableProperty]
            private bool state;

            [ObservableProperty]
            private int wafer_number;

            [ObservableProperty]
            private List<int> wafer_List;

            [ObservableProperty]
            private List<int> vIDItems;

            [ObservableProperty]
            private int loadport_Number;

            private List<int> rPTIDs;
            public List<int> RPTIDs
            {
                get => rPTIDs;
                set
                {
                    if (SetProperty(ref rPTIDs, value))
                    {
                        OnPropertyChanged(nameof(RPTIDsDisplay));
                    }
                }
            }

            public string? RPTIDsDisplay
            {
                get => rPTIDs != null ? string.Join(", ", rPTIDs) : string.Empty;
                set
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        rPTIDs = value
                            .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => int.TryParse(s, out int n) ? n : -1)
                            .Where(n => n >= 0)
                            .ToList();
                        OnPropertyChanged(nameof(RPTIDsDisplay));
                    }
                }
            }
        }

        public class SVIDInfo
        {
            public int Number { get; set; }
            public string Name { get; set; }     // 예: Temperature
            public string Description { get; set; }  // 예: "Current chamber temperature"
            public string Type { get; set; }     // 예: "FLOAT", "ASCII", "INT"
        }
    }
}
