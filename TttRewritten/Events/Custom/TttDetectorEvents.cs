using System;

using TttRewritten.Enums;

namespace TttRewritten.Events.Custom
{
    public static class TttDetectorEvents
    {
        public static event Action<TttPlayer, TttRoleType> OnRoleRevealed;

        internal static void TriggerOnRoleRevealed(TttPlayer player)
            => OnRoleRevealed?.Invoke(player, player.RoleType);
    }
}