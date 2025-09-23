using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Secs4Net;
using SemiConductor_Equipment.Enums;
using SemiConductor_Equipment.Models;

namespace SemiConductor_Equipment.interfaces
{
    public interface IMessageManager
    {
        #region PROPERTIES
        #endregion

        #region METHODS
        Task HandleMessageAsync(PrimaryMessageWrapper wrapper);
        #endregion
    }
}
