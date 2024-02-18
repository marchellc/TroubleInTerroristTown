using PlayerStatsSystem;

using System;

namespace TttRewritten.Events.Custom
{
    public static class TttPlayerEvents
    {
        public static event Action<TttPlayer> OnJoined;
        public static event Action<TttPlayer> OnLeft;

        public static event Action<TttPlayer, TttPlayer, DamageHandlerBase> OnDied;

        internal static void TriggerOnJoined(TttPlayer player)
            => OnJoined?.Invoke(player);

        internal static void TriggerOnLeft(TttPlayer player)
            => OnLeft?.Invoke(player);

        internal static void TriggerOnDied(TttPlayer target, TttPlayer killer, DamageHandlerBase damageHandler)
            => OnDied?.Invoke(target, killer, damageHandler);
    }
}