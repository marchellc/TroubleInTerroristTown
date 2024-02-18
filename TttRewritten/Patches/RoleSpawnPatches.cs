using helpers.Patching;

using PlayerRoles.RoleAssign;

namespace TttRewritten.Patches
{
    public static class RoleSpawnPatches
    {
        [Patch(typeof(RoleAssigner), nameof(RoleAssigner.OnRoundStarted))]
        public static bool DisableAssigner()
            => false;
    }
}