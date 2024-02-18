using TttRewritten.Enums;
using TttRewritten.Configs;
using TttRewritten.Roles.Implemented;
using TttRewritten.Events.Custom;

using PlayerRoles;

using MEC;

using PlayerStatsSystem;

using Compendium.Features;

namespace TttRewritten.Roles
{
    public class TttRoleManager
    {
        static TttRoleManager()
        {
            TttPlayerEvents.OnDied += OnPlayerDied;
        }

        public TttRoleManager(TttPlayer player)
            => Player = player;

        public TttPlayer Player { get; }
        public TttRole Role { get; private set; }

        public bool IsMurderer
        {
            get => Role.Type is TttRoleType.Murderer;
        }

        public bool IsInnocent
        {
            get => Role.Type is TttRoleType.Innocent;
        }

        public bool IsDetective
        {
            get => Role.Type is TttRoleType.Detective;
        }

        public bool IsDead
        {
            get => Role.Type is TttRoleType.Dead;
        }

        public bool IsSpawned
        {
            get => Role.Type != TttRoleType.None;
        }

        public void Set(TttRoleType type, TttSpawnReason spawnReason)
        {
            switch (type)
            {
                case TttRoleType.None:
                    SetRole(new DefaultRole(), spawnReason);
                    break;

                case TttRoleType.Dead:
                    SetRole(new DeadRole(), spawnReason);
                    break;

                case TttRoleType.Detective:
                    SetRole(new DetectiveRole(), spawnReason);
                    break;

                case TttRoleType.Innocent:
                    SetRole(new InnocentRole(), spawnReason);
                    break;

                case TttRoleType.Murderer:
                    SetRole(new MurdererRole(), spawnReason);
                    break;

                default:
                    FLog.Error($"Unknown role: {type}!");
                    break;
            }
        }

        internal void SetRole(TttRole tttRole, TttSpawnReason spawnReason)
        {
            FLog.Info($"Setting role of '{Player.GameName}' to {tttRole.Type} ({spawnReason}) ..");

            if (Role != null && Role.Player != null)
                Role.OnDisabled();

            Player.ReferenceHub.roleManager.ServerSetRole(tttRole.SubType, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);

            Role = tttRole;
            Role.Player = Player;

            Timing.CallDelayed(0.1f, () =>
            {
                Role.OnEnabled();
                Timing.CallDelayed(0.1f, () => Role.OnSpawned(spawnReason));
            });

            TttRoleConfigs.DisplayRoleDescription(Player);
        }

        private static void OnPlayerDied(TttPlayer target, TttPlayer attacker, DamageHandlerBase damageHandler)
        {
            target.Role.OnDied();

            if (attacker != null)
            {
                attacker.RoleManager.Role.OnKilled(target);

                if (attacker == target && attacker.RoleManager.IsDetective)
                    target.RoleManager.Set(TttRoleType.Dead, TttSpawnReason.KilledInnocent);
                else if (attacker.RoleManager.IsDetective)
                    target.RoleManager.Set(TttRoleType.Dead, TttSpawnReason.KilledByDetective);
                else if (attacker.RoleManager.IsMurderer)
                    target.RoleManager.Set(TttRoleType.Dead, TttSpawnReason.KilledByMurderer);
                else
                    target.RoleManager.Set(TttRoleType.Dead, TttSpawnReason.RoundStart);
            }
            else
            {
                target.RoleManager.Set(TttRoleType.Dead, TttSpawnReason.RoundStart);
            }
        }
    }
}