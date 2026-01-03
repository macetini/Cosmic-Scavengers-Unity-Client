using System;
using CosmicScavengers.Core.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Responses.Data
{
    public abstract class BaseBinaryResponse : BaseResponse<byte[]>
    {
        public virtual NetworkBinaryCommand Command
        {
            get => throw new NotImplementedException();
        }
        public abstract void Handle(byte[] parameters);
    }
}
