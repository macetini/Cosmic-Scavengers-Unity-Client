namespace CosmicScavengers.Core.Networking.Request.Data.Meta
{
    public interface INetworkRequest
    {
        /// <summary>
        /// Executes the request logic.
        /// </summary>
        /// <param name="parameters">
        /// Variable arguments required for the specific request
        /// (e.g., PlayerID, EntityIDs, TargetPosition).
        /// </param>
        void Execute(params object[] parameters);

        void OnDestroy();
    }
}
