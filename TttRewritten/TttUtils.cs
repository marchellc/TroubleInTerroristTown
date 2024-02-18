using Compendium.Features;

using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;

using Mirror;

using PlayerRoles.Ragdolls;
using PlayerStatsSystem;

namespace TttRewritten
{
    public static class TttUtils
    {
        public static void AddFirearm(this TttPlayer player, ItemType firearm, ItemType ammo)
        {
            var firerm = (Firearm)player.AddItem(firearm);

            firerm.Status = new FirearmStatus(byte.MaxValue, firerm.Status.Flags | FirearmStatusFlags.MagazineInserted, firerm.GetCurrentAttachmentsCode());

            player.SetAmmo(ItemType.Ammo762x39, ushort.MaxValue);

            FLog.Info($"Added firearm '{firearm}' to {player.Nickname} (ammo: {ammo})");
        }

        public static void SpawnRagdoll(this TttPlayer player)
        {
            if (player.ReferenceHub.roleManager.CurrentRole is not IRagdollRole ragdollRole)
                return;

            var ragdollGo = UnityEngine.Object.Instantiate(ragdollRole.Ragdoll.gameObject);

            if (!ragdollGo.TryGetComponent<BasicRagdoll>(out var ragdoll))
                return;

            ragdoll.NetworkInfo = new RagdollData(player.ReferenceHub, new UniversalDamageHandler(-1f, DeathTranslations.BulletWounds), ragdollGo.transform.localPosition, ragdollGo.transform.localRotation);

            NetworkServer.Spawn(ragdoll.gameObject);

            FLog.Info($"Spawned ragdoll of role '{ragdoll.NetworkInfo.RoleType}' (player: {player.Nickname})");
        }
    }
}