using PlayerRoles;

using TttRewritten.Enums;

using UnityEngine;

namespace TttRewritten.Roles
{
    public class TttRole
    {
        public TttPlayer Player { get; set; }

        public virtual TttRoleType Type { get; } 
        public virtual RoleTypeId SubType { get; }

        public virtual Color Color { get; }

        public virtual void OnSpawned(TttSpawnReason spawnReason) { }
        public virtual void OnDied() { }
        public virtual void OnKilled(TttPlayer player) { }

        public virtual void OnEnabled() { }
        public virtual void OnDisabled() { }

        public void SelectSpawn()
            => Player.Position = TttMap.GetSpawnPosition();
    }
}