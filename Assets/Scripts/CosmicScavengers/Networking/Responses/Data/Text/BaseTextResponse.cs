using System;
using CosmicScavengers.Networking.Commands.Text;
using UnityEngine;

namespace CosmicScavengers.Networking.Responses.Data.Text
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
