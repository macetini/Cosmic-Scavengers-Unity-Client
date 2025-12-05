using UnityEngine;

public class PlayerEntity
{
    public long Id { get; set; }
    public long PlayerId { get; set; }    
    public long WorldId { get; set; }
    public string EntityType { get; set; }
    public int ChunkX { get; set; }
    public int ChunkY { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public int Health { get; set; }
    //public string stateData;
    //Instant createdAt
}
