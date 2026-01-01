using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Requests.Channels;
using UnityEngine;

public class LoginPassHandler : MonoBehaviour
{
    public bool Active = true;

    [Header("Channel Configuration")]
    [SerializeField]
    [Tooltip("Channel to send binary requests.")]
    private BinaryRequestChannel requestChanel;

    public NetworkTextCommand Command => NetworkTextCommand.S_LOGIN_PASS;

    void Awake()
    {
        if (requestChanel == null)
        {
            Debug.LogError("[LoginPassHandler] BinaryCommandChannel reference is missing!");
        }
    }

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

        /*requestChanel.Raise(NetworkBinaryCommand.REQUEST_WORLD_STATE_C, new object[] { playerId });

        requestChanel.Raise(
            NetworkBinaryCommand.REQUEST_PLAYER_ENTITIES_C,
            new object[] { playerId }
        );
        */
    }
}
