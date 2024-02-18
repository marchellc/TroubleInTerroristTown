using Compendium.Events;
using Compendium.Value;

using PluginAPI.Events;

using TttRewritten.Roles.Implemented;
using TttRewritten.Enums;

using System.Linq;

namespace TttRewritten.Events
{
    public static class ItemEvents
    {
        [Event]
        public static void OnPickedUpItem(PlayerSearchedPickupEvent ev, ValueReference isAllowed)
        {
            isAllowed.Value = false;

            FixEvent(ev);

            var player = TttPlayer.Get(ev.Player);

            if (player is null || !player.RoleManager.IsInnocent)
                return;

            if (TttCoinSpawner.SpawnedCoins.Contains(ev.Item.Info.Serial))
            {
                TttCoinSpawner.SpawnedCoins.Remove(ev.Item.Info.Serial);

                ev.Item.DestroySelf();

                if (ev.Player.Items.Count(it => it.ItemTypeId is ItemType.Coin) >= 7)
                {
                    player.ClearInventory();
                    player.AddFirearm(ItemType.GunCOM18, ItemType.Ammo9x19);
                    player.Show(
                        $"<b>Obdržel jsi <color=#ff0000>COM-18</color> za posbírání 8 coinů!</b>", 8f);
                }
                else
                {
                    player.AddItem(ItemType.Coin);
                }

                return;
            }

            if (InnocentRole.InnocentDrops.Contains(ev.Item.Info.Serial))
            {
                InnocentRole.InnocentDrops.Remove(ev.Item.Info.Serial);
                ev.Item.DestroySelf();
                player.AddFirearm(ItemType.GunCOM18, ItemType.Ammo9x19);
                return;
            }

            if (DetectiveRole.DetectiveGuns.Contains(ev.Item.Info.Serial))
            {
                DetectiveRole.DetectiveGuns.Remove(ev.Item.Info.Serial);
                ev.Item.DestroySelf();
                player.RoleManager.Set(TttRoleType.Detective, TttSpawnReason.PickedUpDetectiveGun);
                return;
            }
        }

        private static void FixEvent(PlayerSearchedPickupEvent ev)
        {
            var info = ev.Item.Info;

            info.Locked = false;
            info.InUse = false;

            ev.Item.NetworkInfo = info;
        }
    }
}