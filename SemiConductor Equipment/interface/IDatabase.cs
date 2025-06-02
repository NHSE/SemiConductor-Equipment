using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiConductor_Equipment.interfaces
{
    public interface IDatabase<T>
    {
        // 테이블에 대한 모든 데이터 조회
        List<T>? Get();

        // 테이블에 특정 Data 삽입
        void Create(T entity);

        // 테이블에 특정 Data 업데이트
        void Update(T entity);

        // 테이블에 특정 Data 삭제
        void Delete(int? id);

        // 테이블에 특정 Data 삭제
        List<T>? Search(string? chamberName, DateTime? logTime=null);
    }
}
