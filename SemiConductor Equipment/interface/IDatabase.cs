using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public interface IDatabase<T>
    {
        #region PROPERTIES
        #endregion

        #region METHODS
        List<T>? Get();
        void Create(T entity);
        void Update(T entity);
        void Delete(int? id);
        List<T>? Search(string? chamberName, DateTime? logTime = null);
        List<string>? SearchChamberField(string chamberFieldName);
        #endregion

        #region EVENTS
        #endregion
    }
}
