namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// Contract for all menu / presentation layer implementations.
    ///
    /// UC15: Extracting this interface allows Program.cs (the entry point) to depend
    /// on an abstraction rather than a concrete class, following the
    /// Dependency Inversion Principle.  Future UIs (web, CLI, MAUI) simply implement
    /// this interface and the entry point needs no changes.
    /// </summary>
    public interface IMenu
    {
        /// <summary>Starts the interactive loop until the user exits.</summary>
        void Run();
    }
}
