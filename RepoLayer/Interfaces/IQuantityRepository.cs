using System.Collections.Generic;
using ModelLayer.Entities;

namespace RepoLayer.Interfaces
{
    public interface IQuantityRepository
    {
        void Save(QuantityMeasurementEntity entity);
        IReadOnlyList<QuantityMeasurementEntity> GetAll();
        IReadOnlyList<QuantityMeasurementEntity> GetByOperation(string operationType);
        IReadOnlyList<QuantityMeasurementEntity> GetByCategory(string category);
        int GetTotalCount();
        void DeleteAll();
    }
}
