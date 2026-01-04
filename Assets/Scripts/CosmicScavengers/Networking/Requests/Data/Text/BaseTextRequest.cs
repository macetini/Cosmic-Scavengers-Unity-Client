using System;
using CosmicScavengers.Networking.Commands.Data.Text;
using CosmicScavengers.Networking.Request.Data;

namespace CosmicScavengers.Networking.Request.Text.Data
{
    /// <summary>
    /// Abstract base for binary-serialized network requests.
    /// Inherits from MonoBehaviour to support Unity-native discovery and Inspector configuration.
    /// </summary>
    public abstract class BaseTextRequest : BaseRequest<string>
    {
        protected virtual NetworkTextCommand Command
        {
            get => throw new NotImplementedException();
        }

        protected override void Raise()
        {
            //dispatchChannel.Raise(CommandType.TEXT, Command + "|" + Data);
        }
    }
}
