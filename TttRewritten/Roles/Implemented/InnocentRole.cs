using Compendium;
using Compendium.Attributes;

using InventorySystem.Items.Pickups;

using PlayerRoles;

using System.Collections.Generic;

using TttRewritten.Enums;

using UnityEngine;

namespace TttRewritten.Roles.Implemented
{
    public class InnocentRole : TttRole
    {
        public static readonly HashSet<ushort> InnocentDrops = new HashSet<ushort>();

        public override TttRoleType Type { get; } = TttRoleType.Innocent;
        public override RoleTypeId SubType { get; } = RoleTypeId.Scientist;

        public override Color Color { get; } = new Color(0f, 0.8f, 0f);

        public override void OnEnabled()
        {
            base.OnEnabled();
            SelectSpawn();
        }

        public override void OnDied()
        {
            base.OnDied();

            TttUtils.SpawnRagdoll(Player);

            foreach (var item in Player.Items)
            {
                if (item.ItemTypeId is ItemType.Coin)
                {
                    TttCoinSpawner.Spawn(Player.Position);
                    continue;
                }

                if (item.ItemTypeId is ItemType.GunCOM18)
                {
                    var gun = World.SpawnItem<ItemPickupBase>(ItemType.GunCOM18, Player.Position, Vector3.one, Player.Camera.rotation);
                    InnocentDrops.Add(gun.Info.Serial);
                    continue;
                }
            }
        }

        [RoundStateChanged(Compendium.Enums.RoundState.WaitingForPlayers)]
        private static void OnWaiting()
            => InnocentDrops.Clear();
    }
}