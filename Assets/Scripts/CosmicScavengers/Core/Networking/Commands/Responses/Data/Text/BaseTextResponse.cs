using System;
using CosmicScavengers.Core.Networking.Commands.Data.Text;
using UnityEngine;

namespace CosmicScavengers.Networking.Commands.Responses.Data.Text
{
    public class BaseTextResponse : BaseResponse<string>
    {
        public virtual NetworkTextCommand Command
        {
            get => throw new NotImplementedException();
        }
    }
}
