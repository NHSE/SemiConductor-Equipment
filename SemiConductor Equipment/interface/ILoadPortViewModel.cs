using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemiConductor_Equipment.Models;
using SemiConductor_Equipment.Services;

namespace SemiConductor_Equipment.interfaces
{
    public interface ILoadPortViewModel
    {
        #region PROPERTIES
        byte LoadPortId { get; }  // 로드포트 ID 추가
        #endregion

        #region METHODS
        bool Update_Carrier_info(Wafer newWaferData);

        bool Update_Carrier_Info(Wafer newWaferData, byte slot_num);
        bool Check_Running(string cjid);

        string GetCarrierId();

        string GetPJId(byte loadportId, int wafer_num);

        List<Wafer> GetAllWaferInfo(string pjid);
        #endregion

        #region EVENTS
        #endregion
    }
}
