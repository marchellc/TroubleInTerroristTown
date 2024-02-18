using Compendium.Attributes;
using Compendium.Events;
using Compendium.Value;

using MapGeneration;

using MEC;

using PluginAPI.Events;

using System.Linq;

using TttRewritten.Events.Custom;

using UnityEngine;

namespace TttRewritten
{
    public static class TttDetector
    {
        private static RoomLightController light;

        public static RoomLightController Light
        {
            get
            {
                light ??= RoomLightController.Instances.FirstOrDefault(l => l != null && l.Room != null && l.Room.Name is RoomName.Lcz914);
                return light;
            }
        }

        public static Color Color
        {
            get => Light.NetworkOverrideColor;
            set
            {
                Light.NetworkOverrideColor = value;
                Timing.CallDelayed(10f, () => Light.NetworkOverrideColor = Color.clear);
            }
        }

        [RoundStateChanged(Compendium.Enums.RoundState.WaitingForPlayers)]
        private static void OnWaiting()
            => light = null;

        [Event]
        private static void OnUpgradingItem(Scp914UpgradePickupEvent ev, ValueReference isAllowed)
            => isAllowed.Value = false;

        [Event]
        private static void OnUpgradingInv(Scp914UpgradeInventoryEvent ev, ValueReference isAllowed)
            => isAllowed.Value = false;

        [Event]
        private static void OnProcessPlayer(Scp914ProcessPlayerEvent ev, ValueReference isAllowed)
        {
            isAllowed.Value = false;

            var player = TttPlayer.Get(ev.Player);

            if (player is null)
                return;

            player.Position = ev.OutPosition;

            Color = player.Role.Color;

            TttDetectorEvents.TriggerOnRoleRevealed(player);
        }
    }
}