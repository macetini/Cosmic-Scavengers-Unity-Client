namespace CosmicScavengers.Core.Models
{
    public class WorldData
    {
        public long WorldId { get; set; }
        public string WorldName { get; set; }
        public long MapSeed { get; set; }
        public int SectorSizeUnits { get; set; }

        public override string ToString()
        {
            return $"[WorldState] ID: '{WorldId}', Name: '{WorldName}', Seed: '{MapSeed}', Sector Size Units: '{SectorSizeUnits}'";
        }
    }
}