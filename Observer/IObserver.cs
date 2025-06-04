namespace Kaiju.Observer
{
    /// <summary>
    /// An observer that can receive updates from an observable subject.
    /// Used in the Observer design pattern to notify dependent components of change.
    /// </summary>
    public interface IObserver
    {
        /// <summary>
        /// Called when the subject being observed changes.
        /// Classes that use this define how they react to the update.
        /// </summary>
        public void Updated();
    }
}
