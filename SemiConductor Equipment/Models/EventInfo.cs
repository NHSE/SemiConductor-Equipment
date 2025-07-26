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
        public class RPTIDInfo
        {
            public int Number { get; set; }
            public bool State { get; set; }
            public List<int> CEIDs { get; set; }
        }

        public partial class CEIDInfo : ObservableObject
        {
            [ObservableProperty]
            private int number;

            [ObservableProperty]
            private string name;

            [ObservableProperty]
            private bool state;

            private List<int> sVIDs;
            public List<int> SVIDs
            {
                get => sVIDs;
                set
                {
                    if (SetProperty(ref sVIDs, value))
                    {
                        OnPropertyChanged(nameof(SVIDsDisplay));
                    }
                }
            }

            public string? SVIDsDisplay
            {
                get => SVIDs != null ? string.Join(", ", SVIDs) : string.Empty;
                set
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        SVIDs = value
                            .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => int.TryParse(s, out int n) ? n : -1)
                            .Where(n => n >= 0)
                            .ToList();
                        OnPropertyChanged(nameof(SVIDsDisplay));
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
