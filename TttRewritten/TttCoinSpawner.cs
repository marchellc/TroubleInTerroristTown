using Compendium;
using Compendium.Attributes;

using helpers.Random;

using InventorySystem.Items.Pickups;

using System;
using System.Collections.Generic;

using TttRewritten.Events.Custom;

using UnityEngine;

namespace TttRewritten
{
    public static class TttCoinSpawner
    {
        private static DateTime coinTime;

        public static readonly HashSet<ushort> SpawnedCoins = new HashSet<ushort>();

        public static void Spawn(Vector3 spawnPos)
        {
            var spawnScale = Vector3.one * 2f;
            var spawnRot = new Quaternion(RandomGeneration.Default.GetRandom(0f, 360f), RandomGeneration.Default.GetRandom(0f, 360f), RandomGeneration.Default.GetRandom(0f, 360f), RandomGeneration.Default.GetRandom(0f, 360f));
            var item = World.SpawnItem<ItemPickupBase>(ItemType.Coin, spawnPos, spawnScale, spawnRot);

            SpawnedCoins.Add(item.Info.Serial);

            coinTime = DateTime.Now.AddSeconds(RandomGeneration.Default.GetRandom(10, 30));
        }

        internal static void Init()
            => TttRoundEvents.OnUpdate += OnUpdate;

        private static void OnUpdate()
        {
            if (DateTime.Now > coinTime)
                Spawn(TttMap.GetSpawnPosition(false));
        }

        [RoundStateChanged(Compendium.Enums.RoundState.WaitingForPlayers)]
        private static void OnWaiting()
            => SpawnedCoins.Clear();
    }
}