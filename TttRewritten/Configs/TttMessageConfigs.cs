using helpers.Configuration;

using MapGeneration;

using PluginAPI.Core;

using System.Linq;

using TttRewritten.Enums;
using TttRewritten.Events.Custom;

namespace TttRewritten.Configs
{
    public static class TttMessageConfigs
    {
        static TttMessageConfigs()
        {
            TttPlayerEvents.OnJoined += OnPlayerJoined;

            TttRoundEvents.OnEnding += OnRoundEnding;

            TttMapEvents.OnLockingZone += OnLockingZone;
            TttMapEvents.OnLockedZone += OnLockedZone;

            TttDetectorEvents.OnRoleRevealed += OnRoleRevealed;
        }

        [Config(Name = "Welcome Message", Description = "The message to display when a player joins.")]
        public static TttMessage WelcomeMessage { get; set; } = new TttMessage
        {
            Duration = 10f,
            Message = "Vítej na Trouble in Terrorist Town serveru!"
        };

        [Config(Name = "Locking Message", Description = "The message to display when a facility zone starts locking up.")]
        public static TttMessage LockingMessage { get; set; } = new TttMessage
        {
            Duration = 10f,
            Message = "Zóna %zone% se uzavírá!"
        };

        [Config(Name = "Locked Message", Description = "The message to display when a zone locks up.")]
        public static TttMessage LockedMessage { get; set; } = new TttMessage
        {
            Duration = 5f,
            Message = "Zóna %zone% je nyní zamknuta."
        };

        [Config(Name = "Reveal Message", Description = "The message to display when someone uses SCP-914 as a role reveal.")]
        public static TttMessage RevealMessage { get; set; } = new TttMessage
        {
            Duration = 5f,
            Message = "%name% je %role%!"
        };

        public static void DisplayRoleRevealMessage(string revealedName, TttRoleType revealedRole)
        {
            var room = RoomIdentifier.AllRoomIdentifiers.FirstOrDefault(r => r.Name is RoomName.Lcz914);

            if (room is null)
                return;

            RevealMessage.SendDistance(room.transform.position, 10f, msg => msg.Replace("%name%", revealedName).Replace("%role%", revealedRole.GetColoredRoleName()));
        }

        public static void DisplayRoundEndMessage(TttRoleType winner)
        {
            var winnerName = winner.GetColoredRoleName();

            foreach (var player in TttPlayer.Players)
            {
                player.Show(
                    $"<size=70><color=#ff0000><b>[</b></color><color=#00ffbe><b>KONEC KOLA</b></color><color=#ff0000><b>]</b></color>\n" +
                    $"► <color=#9eff00>Vítězící strana</color>: <b>{winnerName}</b>\n" +
                    $"► <color=#9eff00>Délka kola</color>: <b>{Round.Duration}</b>", TttRoundConfigs.RestartDelay < 0f ? 15f : TttRoundConfigs.RestartDelay);
            }
        }

        public static void DisplayZoneLockedMessage(FacilityZone zone)
        {
            if (LockedMessage is null || !LockedMessage.IsValid)
                return;

            string zoneName = zone switch
            {
                FacilityZone.Entrance         => "EZ",
                FacilityZone.HeavyContainment => "HCZ",
                FacilityZone.LightContainment => "LCZ",

                _ => "ZONE"
            };

            LockedMessage.Send(msg => msg.Replace("%zone%", zoneName), TttPlayer.Players.ToArray());
        }

        public static void DisplayZoneLockingMessage(FacilityZone zone)
        {
            if (LockingMessage is null || !LockingMessage.IsValid)
                return;

            string zoneName = zone switch
            {
                FacilityZone.Entrance         => "EZ",
                FacilityZone.HeavyContainment => "HCZ",
                FacilityZone.LightContainment => "LCZ",

                _ => "ZONE"
            };

            foreach (var player in TttPlayer.Players)
            {
                if (!player.RoleManager.IsSpawned || player.RoleManager.IsDead)
                    continue;

                var plyZone = player.Zone;

                if (plyZone != zone)
                    continue;

                LockingMessage.Send(msg => msg.Replace("%zone%", zoneName), player);
            }
        }

        public static void DisplayWelcomeMessage(TttPlayer player)
        {
            if (WelcomeMessage != null && WelcomeMessage.IsValid)
                WelcomeMessage.Send(msg => msg.Replace("%name%", player.GameName), player);
        }

        private static void OnPlayerJoined(TttPlayer player)
            => DisplayWelcomeMessage(player);

        private static void OnRoundEnding(float _, TttRoleType winner)
            => DisplayRoundEndMessage(winner);

        private static void OnLockingZone(FacilityZone zone)
            => DisplayZoneLockingMessage(zone);

        private static void OnLockedZone(FacilityZone zone)
            => DisplayZoneLockedMessage(zone);

        private static void OnRoleRevealed(TttPlayer player, TttRoleType _)
            => DisplayRoleRevealMessage(player.GameName, player.RoleType);
    }
}