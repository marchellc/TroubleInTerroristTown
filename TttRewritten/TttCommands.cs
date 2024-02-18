using BetterCommands;
using BetterCommands.Permissions;

using MapGeneration;

using TttRewritten.Enums;

namespace TttRewritten
{
    public static class TttCommands
    {
        [Command("force", CommandType.RemoteAdmin, CommandType.GameConsole)]
        [Permission(PermissionLevel.Lowest)]
        public static string ForceRoundCommand(ReferenceHub sender)
        {
            TttRound.IsForced = true;
            return $"Round Start has been forced.";
        }

        [Command("lock", CommandType.RemoteAdmin, CommandType.GameConsole)]
        [Permission(PermissionLevel.Lowest)]
        public static string LockRoundCommand(ReferenceHub sender)
        {
            TttRound.IsLocked = !TttRound.IsLocked;
            return TttRound.IsLocked ? "The round is now locked." : "The round has been unlocked.";
        }

        [Command("end", CommandType.RemoteAdmin, CommandType.GameConsole)]
        [Permission(PermissionLevel.Lowest)]
        public static string EndCommand(ReferenceHub sender, TttRoleType winner)
        {
            TttRound.Winner = winner;
            return $"The round is ending.";
        }

        [Command("lockzone", CommandType.RemoteAdmin, CommandType.GameConsole)]
        [Permission(PermissionLevel.Lowest)]
        public static string LockZoneCommand(ReferenceHub sender, FacilityZone zone)
        {
            if (TttMap.IsLocked(zone))
                return $"That zone is already locked.";

            TttMap.Lock(zone);
            return $"Locking zone {zone} ..";
        }

        [Command("role", CommandType.RemoteAdmin, CommandType.GameConsole)]
        [Permission(PermissionLevel.Lowest)]
        public static string RoleCommand(ReferenceHub sender, ReferenceHub target, TttRoleType role)
        {
            var player = TttPlayer.Get(target);

            if (player is null)
                return "Failed to find player.";

            player.RoleManager.Set(role, TttSpawnReason.RemoteAdmin);
            return "Changed role.";
        }
    }
}
