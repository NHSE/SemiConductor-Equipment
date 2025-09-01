using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public interface IMessageBox
    {
        #region PROPERTIES
        #endregion

        #region METHODS
        void Show(string title, string message);
        #endregion

        #region EVENTS
        event EventHandler<List<string>> Message_Show;
        #endregion
    }
}
