using PlayerRoles;

using TttRewritten.Enums;

using UnityEngine;

namespace TttRewritten.Roles.Implemented
{
    public class DefaultRole : TttRole
    {
        public override TttRoleType Type { get; } = TttRoleType.None;
        public override RoleTypeId SubType { get; } = RoleTypeId.None;

        public override Color Color { get; } = new Color(0.25f, 0.75f, 0.5f);
    }
}