using System;
using CosmicScavengers.Networking.Commands.Data.Binary;

namespace CosmicScavengers.Networking.Responses.Data.Binary
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
