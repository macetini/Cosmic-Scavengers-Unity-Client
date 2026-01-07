using System;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;

namespace CosmicScavengers.Networking.Commands.Responses.Data.Binary
{
    public abstract class BaseBinaryResponse : BaseResponse<byte>
    {
        public virtual NetworkBinaryCommand Command
        {
            get => throw new NotImplementedException();
        }
    }
}
