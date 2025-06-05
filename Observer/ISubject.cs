namespace Kaiju.Observer
{
    /// <summary>
    /// Represents a subject in the Observer pattern.
    /// A subject maintains a list of observers and notifies them of any changes to its state.
    /// </summary>
    public interface ISubject
    {
        /// <summary>
        /// Attaches (or subscribes) an observer to receive updates from this subject.
        /// </summary>
        /// <param name="observer">The observer to attach</param>
        public void Attach(IObserver observer);

        /// <summary>
        /// Detaches (or unsubscribes) an observer from receiving updates.
        /// </summary>
        /// <param name="observer">The observer to detach</param>
        public void Detach(IObserver observer);

        /// <summary>
        /// Notifies all attached observers of a change in the subject's state.
        /// </summary>
        public void Notify();
    }
}
