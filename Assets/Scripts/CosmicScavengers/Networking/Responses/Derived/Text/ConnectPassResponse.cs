using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Responses.Data;
using UnityEngine;

public class ConnectPassResponse : BaseTextResponse
{
    public override NetworkTextCommand Command => NetworkTextCommand.S_CONNECT_PASS;

    public override void Handle(string[] parameters)
    {
        Debug.Log(
            "[ConnectPassResponse] Handshake with server completed. Ready for login/register."
        );
    }
}
