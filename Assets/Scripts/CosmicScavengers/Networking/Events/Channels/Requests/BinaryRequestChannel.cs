using CosmicScavengers.Core.Event;
using CosmicScavengers.Core.Networking.Commands;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/Requests/BinaryRequestChannel")]
public class BinaryRequestChannel : EventChannel<NetworkBinaryCommand, object[]> { }
