using PluginAPI.Core;
using PluginAPI.Core.Interfaces;

using TttRewritten.Roles;
using TttRewritten.Enums;

using System.Collections.Generic;

using Compendium.Attributes;

using System.Linq;

using MapGeneration;

using TttRewritten.Events.Custom;
using Compendium.Features;

namespace TttRewritten
{
    public class TttPlayer : Player
    {
        private static readonly Dictionary<Player, TttPlayer> playersToPlayers = new Dictionary<Player, TttPlayer>();
        private static readonly Dictionary<ReferenceHub, TttPlayer> hubsToPlayers = new Dictionary<ReferenceHub, TttPlayer>();

        private string gameName;

        public TttPlayer(IGameComponent component) : base(component)
        {
            GameName = TttNicks.GenerateNext();

            RoleManager = new TttRoleManager(this);
            RoleManager.Set(TttRound.IsInProgress ? TttRoleType.Dead : TttRoleType.None, TttSpawnReason.RoundStart);

            FLog.Info($"Player '{Nickname}' joined as {GameName}.");
        }

        public TttRoleManager RoleManager { get; }

        public new TttRole Role
        {
            get => RoleManager.Role;
        }

        public TttRoleType RoleType
        {
            get => RoleManager.Role.Type;
            set => RoleManager.Set(value, TttSpawnReason.RoundStart);
        }

        public string GameName
        {
            get => gameName;
            set => gameName = DisplayNickname = value;
        }

        private void OnLockedZone(FacilityZone zone)
        {
            if (Zone == zone)
            {
                RoleManager.Set(TttRoleType.Dead, TttSpawnReason.KilledByLockdown);

                this.Show(
                    $"<b>Zemřel jsi na <color=#ff0000>zamknutí zóny.</color></b>", 10f);
            }
        }

        private void OnLockingZone(FacilityZone zone)
        {
            if (Zone == zone)
            {
                this.Show(
                    $"<b>Tato zóna se uzavírá! Máš <color=#ff0000>20 sekund</color> na útěk!</color></b>", 10f);
            }
        }

        public static IEnumerable<TttPlayer> Players
        {
            get => playersToPlayers.Values;
        }

        public static IEnumerable<TttPlayer> ValidPlayers
        {
            get => playersToPlayers.Values.Where(p => !p.IsOverwatchEnabled);
        }

        public static TttPlayer Get(Player player)
        {
            if (playersToPlayers.TryGetValue(player, out var tttPlayer))
                return tttPlayer;

            return null;
        }

        public static new TttPlayer Get(ReferenceHub hub)
        {
            if (hubsToPlayers.TryGetValue(hub, out var tttPlayer))
                return tttPlayer;

            return null;
        }

        internal static void Include(TttPlayer player, Player playerBase)
        {
            playersToPlayers[playerBase] = player;
            hubsToPlayers[playerBase.ReferenceHub] = player;

            TttMapEvents.OnLockingZone += player.OnLockingZone;
            TttMapEvents.OnLockedZone += player.OnLockedZone;
        }

        internal static void Destroy(Player player)
        {
            if (playersToPlayers.TryGetValue(player, out var tttPlayer))
            {
                tttPlayer.Role.OnDied();
                tttPlayer.Role.OnDisabled();

                TttMapEvents.OnLockingZone -= tttPlayer.OnLockingZone;
                TttMapEvents.OnLockedZone -= tttPlayer.OnLockedZone;
            }

            playersToPlayers.Remove(player);
            hubsToPlayers.Remove(player.ReferenceHub);
        }

        [RoundStateChanged(Compendium.Enums.RoundState.WaitingForPlayers)]
        internal static void ClearPlayers()
        {
            playersToPlayers.Clear();
            hubsToPlayers.Clear();
        }
    }
}