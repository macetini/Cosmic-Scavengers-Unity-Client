using System;
using CosmicScavengers.Core.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Responses.Data
{
    public class BaseTextResponse : MonoBehaviour
    {
        public virtual NetworkTextCommand Command
        {
            get => throw new NotImplementedException();
        }

        public virtual void Handle(string[] parameters) { }

        public virtual void OnDestroy() { }
    }
}
