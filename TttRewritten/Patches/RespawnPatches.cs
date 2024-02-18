using helpers.Patching;

using Respawning;

namespace TttRewritten.Patches
{
    public static class RespawnPatches
    {
        [Patch(typeof(RespawnManager), nameof(RespawnManager.Update), PatchType.Prefix)]
        public static bool PreventRespawn()
            => false;
    }
}