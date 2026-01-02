using UnityEngine;

namespace CosmicScavengers.Core.Networking.Responses.Data
{
    public abstract class BaseBinaryResponse : BaseResponse<byte[]>
    {
        public abstract void Handle(byte[] parameters);
    }
}
