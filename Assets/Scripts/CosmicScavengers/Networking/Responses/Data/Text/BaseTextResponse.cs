using System;
using CosmicScavengers.Networking.Commands.Data.Text;
using UnityEngine;

namespace CosmicScavengers.Networking.Responses.Data.Text
{
    public class BaseTextResponse : BaseResponse<string>
    {
        public virtual NetworkTextCommand Command
        {
            get => throw new NotImplementedException();
        }
    }
}
