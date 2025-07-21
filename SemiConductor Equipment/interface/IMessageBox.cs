using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public interface IMessageBox
    {
        void Show(string title, string message);

        event EventHandler<List<string>> Message_Show;
    }
}
