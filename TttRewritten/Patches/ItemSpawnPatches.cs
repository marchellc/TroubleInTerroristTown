using helpers.Patching;

using InventorySystem;
using InventorySystem.Items.Usables.Scp244;

using MapGeneration.Distributors;

namespace TttRewritten.Patches
{
    public static class ItemSpawnPatches
    {
        [Patch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropAmmo), PatchType.Prefix)]
        public static bool DisableAmmoSpawnPatch()
            => false;

        [Patch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropItem), PatchType.Prefix)]
        public static bool DisableItemSpawnPatch()
            => false;

        [Patch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropEverything), PatchType.Prefix)]
        public static bool DisableItemSpawnPatch2()
            => false;

        [Patch(typeof(ItemDistributor), nameof(ItemDistributor.PlaceSpawnables), PatchType.Prefix)]
        public static bool DisableItemSpawnPatch3()
            => false;

        [Patch(typeof(Scp244Spawner), nameof(Scp244Spawner.SpawnScp244), PatchType.Prefix)]
        public static bool DisableScp244SpawnPatch()
            => false;
    }
}