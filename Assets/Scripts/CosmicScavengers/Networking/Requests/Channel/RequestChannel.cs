using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Networking.Channel
{
    /// <summary>
    /// A unified channel for all outgoing request.
    /// Perfectly compatible with EventChannel<T1, T2> logic.
    /// </summary>
    [CreateAssetMenu(
        fileName = "RequestChannel",
        menuName = "CosmicScavengers/Channels/RequestChannel"
    )]
    public class RequestChannel : EventChannel<BaseNetworkCommand, object[]>
    {
        /// <summary>
        /// Raise a command with zero arguments.
        /// Passes null to the object[] parameter.
        /// </summary>
        public void Raise(BaseNetworkCommand command)
        {
            base.Raise(command, null);
        }

        /// <summary>
        /// Raise a command with exactly one argument.
        /// Packs the argument into a temporary object array.
        /// </summary>
        public void Raise(BaseNetworkCommand command, object arg)
        {
            base.Raise(command, new object[] { arg });
        }

        /// <summary>
        /// Raise a command with two arguments.
        /// Packs both arguments into a temporary object array.
        /// </summary>
        public void Raise(BaseNetworkCommand command, object arg1, object arg2)
        {
            base.Raise(command, new object[] { arg1, arg2 });
        }

        /// <summary>
        /// Fallback for more complex requests.
        /// Uses params to simplify the calling syntax.
        /// </summary>
        public void RaiseArgs(BaseNetworkCommand command, params object[] args)
        {
            base.Raise(command, args);
        }
    }
}
