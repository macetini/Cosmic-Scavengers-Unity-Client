using System;
using CosmicScavengers.Networking.Channel.Data;
using CosmicScavengers.Networking.Commands.Data.Text;
using CosmicScavengers.Networking.Request.Data;
using UnityEngine;

namespace CosmicScavengers.Networking.Request.Text.Data
{
    /// <summary>
    /// Abstract base for binary-serialized network requests.
    /// Inherits from MonoBehaviour to support Unity-native discovery and Inspector configuration.
    /// </summary>
    public abstract class BaseTextRequest : BaseRequest<string>
    {
        protected string[] Data;
        protected virtual NetworkTextCommand Command
        {
            get => throw new NotImplementedException();
        }

        protected override void Raise()
        {
            if (Data == null)
            {
                Data = new string[0];
                Data[0] = "";
                Debug.LogWarning(
                    $"[{gameObject.name}] Data is not assigned in BaseRequest, using empty string."
                );
            }

            ChannelData channelData = new(Data);
            requestChannel.Raise(Command, channelData);
        }
    }
}
