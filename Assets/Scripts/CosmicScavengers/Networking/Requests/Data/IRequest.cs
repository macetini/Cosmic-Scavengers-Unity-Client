namespace CosmicScavengers.Networking.Requests.Base
{
    /// <summary>
    /// Defines a contract for any network request that can be executed from the client.
    /// This allows UI and Game Logic to trigger requests without knowing the underlying implementation.
    /// </summary>
    /// <typeparam name="T">The type of parameters the request accepts.</typeparam>
    public interface IRequest<T>
    {
        /// <summary>
        /// Whether the request is currently allowed to be sent.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Triggers the network request with the provided parameters.
        /// </summary>
        void Execute(T[] parameters);
    }
}
