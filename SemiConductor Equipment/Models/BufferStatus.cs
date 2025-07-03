using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.Models
{
    public partial class BufferStatus
    {
        public string BufferName { get; }
        public string State { get; }

        public BufferStatus(string value1, string value2)
        {
            BufferName = value1;
            State = value2;
        }
    }
}
