using Compendium.Attributes;

using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Pickups;

using PlayerRoles;

using System.Collections.Generic;

using TttRewritten.Configs;
using TttRewritten.Enums;

using UnityEngine;

namespace TttRewritten.Roles.Implemented
{
    public class DetectiveRole : TttRole
    {
        public static HashSet<ushort> DetectiveGuns { get; } = new HashSet<ushort>();

        public override TttRoleType Type { get; } = TttRoleType.Detective;
        public override RoleTypeId SubType { get; } = RoleTypeId.NtfCaptain;

        public override Color Color { get; } = new Color(0f, 0f, 0.8f);

        public override void OnSpawned(TttSpawnReason spawnReason)
        {
            base.OnSpawned(spawnReason);

            if (spawnReason != TttSpawnReason.PickedUpDetectiveGun)
                SelectSpawn();

            Player.AddFirearm(ItemType.GunE11SR, ItemType.Ammo556x45);
        }

        public override void OnDied()
        {
            base.OnDied();

            var spawnPos = Player.Position;
            var spawnRot = Player.Rotation;

            var gun = TttMap.Spawn<FirearmPickup>(ItemType.GunE11SR, spawnPos, Vector3.one, Quaternion.Euler(spawnRot), true, firearm =>
            {
                firearm.NetworkStatus = new FirearmStatus(byte.MaxValue, firearm.Status.Flags | FirearmStatusFlags.MagazineInserted, AttachmentsUtils.GetRandomAttachmentsCode(ItemType.GunE11SR));
            });

            DetectiveGuns.Add(gun.Info.Serial);

            TttUtils.SpawnRagdoll(Player);
        }

        public override void OnKilled(TttPlayer player)
        {
            base.OnKilled(player);

            if (!player.RoleManager.IsMurderer)
            {
                var spawnPos = Player.Position;
                var spawnRot = Player.Rotation;

                var gun = TttMap.Spawn<FirearmPickup>(ItemType.GunE11SR, spawnPos, Vector3.one, Quaternion.Euler(spawnRot), true, firearm =>
                {
                    firearm.NetworkStatus = new FirearmStatus(byte.MaxValue, firearm.Status.Flags | FirearmStatusFlags.MagazineInserted, AttachmentsUtils.GetRandomAttachmentsCode(ItemType.GunE11SR));
                });

                Player.Show($"<b>Hráč <color=#ff0000>{player.GameName}</color> byl {TttRoleType.Innocent.GetColoredRoleName()}!</b>", 5f);
                Player.RoleManager.Set(TttRoleType.Dead, TttSpawnReason.KilledInnocent);

                DetectiveGuns.Add(gun.Info.Serial);
                return;
            }
        }

        [RoundStateChanged(Compendium.Enums.RoundState.WaitingForPlayers)]
        private static void OnRoundWaiting()
            => DetectiveGuns.Clear();
    }
}