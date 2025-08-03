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
        bool Update_Carrier_info(Wafer newWaferData);

        bool Check_Running(string cjid);

        public string GetCarrierId();

        public string GetPJId(byte loadportId);

        byte LoadPortId { get; }  // 로드포트 ID 추가

        public List<Wafer> GetAllWaferInfo(string pjid);
    }
}
