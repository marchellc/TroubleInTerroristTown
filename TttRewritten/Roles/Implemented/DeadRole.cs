using PlayerRoles;

using TttRewritten.Enums;

using UnityEngine;

namespace TttRewritten.Roles.Implemented
{
    public class DeadRole : TttRole
    {
        public override TttRoleType Type { get; } = TttRoleType.Dead;
        public override RoleTypeId SubType { get; } = RoleTypeId.Spectator;

        public override Color Color { get; } = new Color(0f, 0.5f, 0.5f);
    }
}