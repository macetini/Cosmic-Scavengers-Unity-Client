using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Handlers.Text;
using CosmicScavengers.Networking.Event.Channels;
using UnityEngine;

public class ConnectPassHandler : MonoBehaviour, ITextCommandHandler
{
    public bool Active = true;
    public NetworkTextCommand Command => NetworkTextCommand.S_CONNECT_PASS;

    public TextCommandChannel Channel;

    public void Handle(string[] data)
    {
        if (!Active)
        {
            Debug.Log("[ConnectPassHandler] Handler is inactive. Ignoring message.");
            return;
        }

        Debug.Log("[ConnectPassHandler] Sending login credentials to server.");
        string username = "player_1";
        string password = "secret";

        Channel.Raise($"{NetworkTextCommand.C_LOGIN}|{username}|{password}");
    }
}
