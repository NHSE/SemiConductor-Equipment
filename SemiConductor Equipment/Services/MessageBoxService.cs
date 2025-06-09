using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.interfaces;

namespace SemiConductor_Equipment.Services
{
    internal class MessageBoxService : IMessageBox
    {
        public void Show(string message)
        {
            MessageBox.Show(message);
        }
    }
}
