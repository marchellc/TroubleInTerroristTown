using Compendium.Events;

using PluginAPI.Events;

using TttRewritten.Events.Custom;

namespace TttRewritten.Events
{
    public static class PlayerEvents
    {
        [Event]
        public static void OnPlayerJoined(PlayerJoinedEvent ev)
        {
            var player = new TttPlayer(ev.Player.ReferenceHub);

            TttPlayer.Include(player, ev.Player);
            TttPlayerEvents.TriggerOnJoined(player);
        }

        [Event]
        public static void OnPlayerLeft(PlayerLeftEvent ev)
        {
            var player = TttPlayer.Get(ev.Player);

            if (player != null)
            {
                TttPlayerEvents.TriggerOnLeft(player);
                TttPlayer.Destroy(player);
            }
        }

        [Event]
        public static void OnPlayerDied(PlayerDyingEvent ev)
        {
            var target = ev.Player is null ? null : TttPlayer.Get(ev.Player);
            var attacker = ev.Attacker is null ? null : TttPlayer.Get(ev.Attacker);

            TttPlayerEvents.TriggerOnDied(target, attacker, ev.DamageHandler);
        }
    }
}