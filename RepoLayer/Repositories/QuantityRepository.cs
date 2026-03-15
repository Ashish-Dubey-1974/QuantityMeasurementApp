using ModelLayer.Entities;
using RepoLayer.Interfaces;

namespace RepoLayer.Repositories
{
    /// <summary>
    /// Singleton in-memory cache repository for QuantityMeasurementEntity records.
    ///
    /// UC15:
    ///  - Implements IQuantityRepository (Interface Segregation Principle).
    ///  - Singleton pattern ensures only one cache list exists across the entire
    ///    application, giving a consistent view of operation history.
    ///  - Thread-safe via a private lock object.
    ///
    /// Future extension: replace this class with a DatabaseRepository that also
    /// implements IQuantityRepository — the service layer needs no changes.
    /// </summary>
    public sealed class QuantityRepository : IQuantityRepository
    {
        // ── Singleton ─────────────────────────────────────────────────────────
        private static QuantityRepository? _instance;
        private static readonly object _singletonLock = new object();

        public static QuantityRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_singletonLock)
                    {
                        _instance ??= new QuantityRepository();
                    }
                }
                return _instance;
            }
        }

        // ── Storage ───────────────────────────────────────────────────────────
        private readonly List<QuantityMeasurementEntity> _cache = new();
        private readonly object _writeLock = new object();

        private QuantityRepository() { }

        /// <summary>Appends the entity to the in-memory list.</summary>
        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            lock (_writeLock) { _cache.Add(entity); }
        }

        /// <summary>Returns a read-only snapshot of all stored records.</summary>
        public IReadOnlyList<QuantityMeasurementEntity> GetAll()
        {
            lock (_writeLock) { return _cache.AsReadOnly(); }
        }
    }
}
