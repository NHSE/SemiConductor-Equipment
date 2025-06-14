using System;
using System.Collections.Generic;
using System.Linq;
using SemiConductor_Equipment.interfaces;
using SemiConductor_Equipment.Models;

public class ChamberStatusService : IDatabase<ChamberStatus>
{
    private readonly LogDatabaseContext _db; // 실제 DB 컨텍스트
    public Dictionary<int, ChamberStatus> ChamberStates { get; } = new();

    public ChamberStatusService(LogDatabaseContext db)
    {
        this._db = db;
        for (int i = 1; i <= 6; i++)
        {
            ChamberStates.Add(i, new ChamberStatus());
        }
    }

    public void Create(ChamberStatus entity)
    {
        this._db.ChamberStatuses.Add(entity);
        this._db.SaveChanges();
    }

    public void Delete(int? id)
    {
        var entity = this._db.ChamberStatuses.Find(id);
        if (entity != null)
        {
            this._db.ChamberStatuses.Remove(entity);
            this._db.SaveChanges();
        }
    }

    public List<ChamberStatus>? Get()
    {
        return this._db.ChamberStatuses.ToList();
    }

    public List<ChamberStatus>? Search(string? chamberName, DateTime? logTime = null)
    {
        var query = this._db.ChamberStatuses.AsQueryable();
        if (!string.IsNullOrWhiteSpace(chamberName))
        {
            if (chamberName == "Ch1")
                query = query.Where(c => c.Ch1 != null);
            else if (chamberName == "Ch2")
                query = query.Where(c => c.Ch2 != null);
            else if (chamberName == "Ch3")
                query = query.Where(c => c.Ch3 != null);
            else if (chamberName == "Ch4")
                query = query.Where(c => c.Ch4 != null);
            else if (chamberName == "Ch5")
                query = query.Where(c => c.Ch5 != null);
            else if (chamberName == "Ch6")
                query = query.Where(c => c.Ch6 != null);
        }
        return query.ToList();
    }

    public List<string>? SearchChamberField(string chamberFieldName)
    {
        var query = _db.ChamberStatuses.AsQueryable();

        if (chamberFieldName == "ch1")
            return query.Where(c => c.Ch1 != null).Select(c => c.Ch1).ToList();
        else if (chamberFieldName == "ch2")
            return query.Where(c => c.Ch2 != null).Select(c => c.Ch2).ToList();
        else if (chamberFieldName == "ch3")
            return query.Where(c => c.Ch3 != null).Select(c => c.Ch3).ToList();
        else if (chamberFieldName == "ch4")
            return query.Where(c => c.Ch4 != null).Select(c => c.Ch4).ToList();
        else if (chamberFieldName == "ch5")
            return query.Where(c => c.Ch5 != null).Select(c => c.Ch5).ToList();
        else if (chamberFieldName == "ch6")
            return query.Where(c => c.Ch6 != null).Select(c => c.Ch6).ToList();

        return null;
    }

    public void Update(ChamberStatus entity)
    {
        this._db.ChamberStatuses.Update(entity);
        this._db.SaveChanges();
    }
}
