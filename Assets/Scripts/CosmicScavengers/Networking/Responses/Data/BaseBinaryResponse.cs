using System;
using CosmicScavengers.Networking.Commands;

namespace CosmicScavengers.Networking.Responses.Data
{
    public abstract class BaseBinaryResponse : BaseResponse<byte[]>
    {
        public virtual NetworkBinaryCommand Command
        {
            get => throw new NotImplementedException();
        }
        public abstract void Handle(byte[] data);
    }
}
