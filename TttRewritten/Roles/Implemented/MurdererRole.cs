using MEC;

using PlayerRoles;

using System.Linq;

using TttRewritten.Enums;
using TttRewritten.Events.Custom;

using UnityEngine;

namespace TttRewritten.Roles.Implemented
{
    public class MurdererRole : TttRole
    {
        static MurdererRole()
        {
            TttRoleEvents.OnFinishedAssigningRoles += OnFinishedAssigningRoles;
        }

        public override TttRoleType Type { get; } = TttRoleType.Murderer;
        public override RoleTypeId SubType { get; } = RoleTypeId.Scientist;

        public override Color Color { get; } = new Color(0.8f, 0f, 0f);

        public override void OnEnabled()
        {
            base.OnEnabled();
            SelectSpawn();
        }

        public override void OnSpawned(TttSpawnReason spawnReason)
        {
            base.OnSpawned(spawnReason);

            Player.AddFirearm(ItemType.GunLogicer, ItemType.Ammo762x39);

            if (spawnReason is TttSpawnReason.RemoteAdmin)
                Timing.CallDelayed(0.1f, SendRole);
        }

        public override void OnDied()
        {
            base.OnDied();
            TttUtils.SpawnRagdoll(Player);
        }

        public void SendRole()
        {
            Player.Connection.Send(new RoleSyncInfo(Player.ReferenceHub, RoleTypeId.ClassD, Player.ReferenceHub));

            foreach (var player in TttPlayer.Players.Where(p => p.RoleManager.IsMurderer && p.NetworkId != Player.NetworkId))
                player.Connection.Send(new RoleSyncInfo(Player.ReferenceHub, RoleTypeId.ClassD, player.ReferenceHub));
        }

        private static void OnFinishedAssigningRoles()
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var murderer in TttPlayer.Players.Where(p => p.RoleManager.IsMurderer))
                    (murderer.Role as MurdererRole).SendRole();
            });
        }
    }
}