using Compendium.Attributes;
using Compendium.Extensions;

using helpers.Attributes;

using InventorySystem.Items.Pickups;

using MEC;

using PlayerRoles.Spectating;

using System.Collections.Generic;
using System.Linq;

using TttRewritten.Configs;
using TttRewritten.Enums;
using TttRewritten.Events.Custom;
using TttRewritten.Roles.Implemented;

using UnityEngine;

namespace TttRewritten
{
    public static class TttHints
    {
        private static readonly HashSet<uint> activeHints = new HashSet<uint>();

        [Load]
        internal static void Init()
            => TttRoundEvents.OnUpdate += OnUpdate;

        public static void Show(this TttPlayer player, object content, float time, bool isPriority = false)
        {
            if (!isPriority && activeHints.Contains(player.NetworkId))
            {
                Timing.RunCoroutine(ShowDelayed(player, content, time));
                return;
            }

            player.ReceiveHint(content.ToString(), time);
            activeHints.Add(player.NetworkId);
            Timing.CallDelayed(time, () => activeHints.Remove(player.NetworkId));
        }

        private static IEnumerator<float> ShowDelayed(TttPlayer player, object content, float time)
        {
            while (activeHints.Contains(player.NetworkId))
                yield return Timing.WaitForOneFrame;

            Show(player, content, time);
        }

        private static void OnUpdate()
        {
            foreach (var player in TttPlayer.Players.Where(p => p.IsAlive))
            {
                if (activeHints.Contains(player.NetworkId))
                    continue;

                if (player.IsOverwatchEnabled)
                {
                    var spectated = TttPlayer.Players.FirstOrDefault(p => p.NetworkId != player.NetworkId && p.ReferenceHub.IsSpectatedBy(player.ReferenceHub));

                    if (spectated is null)
                        continue;

                    player.Show(
                        $"\n\n\n<size=70><color=#ff0000>[INFORMACE - {spectated.Nickname}]</color></b>\n" +
                        $"► <color=#ff0000>Jméno</color>: {spectated.GameName}\n" +
                        $"► <color=#ff0000>Role</color>: {spectated.RoleType.GetColoredRoleName()}</size>", 1f);
                }

                if (Physics.Raycast(player.Camera.position, player.Camera.forward, out var hit, 10f, Physics.AllLayers, QueryTriggerInteraction.Ignore)
                    && hit.collider.gameObject.TryGet<ItemPickupBase>(out var pickup)
                    && DetectiveRole.DetectiveGuns.Contains(pickup.Info.Serial))
                {
                    player.Show(
                        $"\n\n<b><color=#ff0000>Tato zbraň patří {TttRoleType.Detective.GetColoredRoleName()}!</color></b>", 5f);
                    continue;
                }

                player.Show(
                    $"\n\n\n<b><color=#ff0000>[HERNÍ INFORMACE]</color></b>\n" +
                    $"► <color=#ff0000>Jméno</color>: {player.GameName}\n" +
                    $"► <color=#ff0000>Role</color>: {player.RoleType.GetColoredRoleName()}", 1.2f);
            }
        }

        [RoundStateChanged(Compendium.Enums.RoundState.WaitingForPlayers)]
        private static void OnWaiting()
            => activeHints.Clear();
    }
}