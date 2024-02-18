using helpers.Configuration;

using MapGeneration;

using System.Collections.Generic;

namespace TttRewritten.Configs
{
    public static class TttMapConfigs
    {
        [Config(Name = "Min EZ Players", Description = "The minimum amount of players required to play in the Entrance Zone.")]
        public static int MinEzPlayers { get; set; } = 12;

        [Config(Name = "Min HCZ Players", Description = "The minimum amount of players required to play in the Heavy Containment Zone.")]
        public static int MinHczPlayers { get; set; } = 6;

        [Config(Name = "Always Locked", Description = "A list of rooms that will be locked.")]
        public static List<RoomName> AlwaysLocked { get; set; } = new List<RoomName>
        {
            RoomName.EzEvacShelter,
            RoomName.EzGateA,
            RoomName.EzGateB,
            RoomName.EzIntercom,
            RoomName.Hcz049,
            RoomName.Hcz079,
            RoomName.Hcz096,
            RoomName.Hcz106,
            RoomName.HczArmory,
            RoomName.HczMicroHID,
            RoomName.HczWarhead,
            RoomName.Lcz173,
            RoomName.Lcz330,
            RoomName.LczArmory,
            RoomName.Outside,
            RoomName.Pocket
        };

        [Config(Name = "Spawn Blacklist", Description = "A list of rooms blacklisted from being used as a spawnpoint.")]
        public static List<RoomName> SpawnBlacklist { get; set; } = new List<RoomName>
        {
            RoomName.EzEvacShelter,
            RoomName.EzGateA,
            RoomName.EzGateB,
            RoomName.EzIntercom,
            RoomName.EzCollapsedTunnel,
            RoomName.EzRedroom,
            RoomName.HczCheckpointA,
            RoomName.HczCheckpointB,
            RoomName.HczCheckpointToEntranceZone,
            RoomName.LczCheckpointA,
            RoomName.LczCheckpointB,
            RoomName.Hcz049,
            RoomName.Hcz079,
            RoomName.Hcz096,
            RoomName.Hcz106,
            RoomName.HczTestroom,
            RoomName.HczArmory,
            RoomName.HczMicroHID,
            RoomName.HczWarhead,
            RoomName.Hcz939,
            RoomName.Lcz173,
            RoomName.Lcz330,
            RoomName.LczArmory,
            RoomName.Outside,
            RoomName.Pocket
        };
    }
}