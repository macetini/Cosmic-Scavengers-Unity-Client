using CosmicScavengers.Networking;
using CosmicScavengers.Networking.Event.Channels;
using CosmicScavengers.Networking.Handlers.Text;
using CosmicScavengers.Networking.Requests;
using UnityEngine;

public class LoginPassHandler : MonoBehaviour, ITextCommandHandler
{
    public bool Active = true;
    public NetworkTextCommand Command => NetworkTextCommand.S_LOGIN_PASS;

    public BinaryCommandChannel Channel;

    public void Handle(string[] data)
    {
        if (!Active)
        {
            Debug.Log("[LoginPassHandler] Handler is inactive. Ignoring message.");
            return;
        }
        if (data.Length < 1)
        {
            Debug.LogError(
                "[LoginPassHandler] Invalid data received. Expected at least 1 parameter."
            );
            return;
        }
        if (!long.TryParse(data[0], out long playerId))
        {
            Debug.LogError("[LoginPassHandler] Failed to parse Player ID from data: " + data[0]);
            return;
        }

        InitPlayerData(playerId);
    }

    private void InitPlayerData(long playerId)
    {
        // Placeholder for any additional initialization logic for player data
        Debug.Log("[LoginPassHandler] Initializing player data for Player ID: " + playerId);

        WorldStateRequest worldStateRequest = new(Channel);
        worldStateRequest.Request(playerId);

        PlayerEntitiesRequest playerEntitiesRequest = new(Channel);
        playerEntitiesRequest.Request(playerId);
    }
}
