using System;
using System.Collections.Generic;
using System.Linq;
using ModelLayer.Entities;
using RepoLayer.Interfaces;

namespace RepoLayer.Repositories
{
    public sealed class QuantityRepository : IQuantityRepository
    {
        private static QuantityRepository? _instance;
        private static readonly object _lock = new object();

        public static QuantityRepository Instance
        {
            get
            {
                if (_instance == null)
                    lock (_lock) { _instance ??= new QuantityRepository(); }
                return _instance;
            }
        }

        private readonly List<QuantityMeasurementEntity> _cache = new();
        private readonly object _writeLock = new object();

        private QuantityRepository() { }

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            lock (_writeLock) { _cache.Add(entity); }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAll()
        {
            lock (_writeLock) { return _cache.AsReadOnly(); }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByOperation(string operationType)
        {
            lock (_writeLock)
            {
                return _cache
                    .Where(e => string.Equals(e.OperationType, operationType, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .AsReadOnly();
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetByCategory(string category)
        {
            lock (_writeLock)
            {
                return _cache
                    .Where(e => string.Equals(e.Operand1?.Category, category, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .AsReadOnly();
            }
        }

        public int GetTotalCount()
        {
            lock (_writeLock) { return _cache.Count; }
        }

        public void DeleteAll()
        {
            lock (_writeLock) { _cache.Clear(); }
        }
    }
}
