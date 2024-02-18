using Compendium;
using Compendium.Attributes;
using Compendium.Extensions;
using Compendium.Features;

using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;

using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Pickups;

using LightContainmentZoneDecontamination;

using MapGeneration;
using MapGeneration.Distributors;

using MEC;

using Mirror;

using PluginAPI.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using TttRewritten.Configs;
using TttRewritten.Events.Custom;

using UnityEngine;

namespace TttRewritten
{
    public static class TttMap
    {
        private static readonly HashSet<Vector3> usedSpawnPositions = new HashSet<Vector3>();

        private static readonly List<DoorVariant> lockedDoors = new List<DoorVariant>();

        private static readonly HashSet<FacilityZone> lockedZones = new HashSet<FacilityZone>();
        private static readonly HashSet<FacilityZone> lockingZones = new HashSet<FacilityZone>();

        static TttMap()
        {
            TttRoundEvents.OnUpdate += ProgressLockdown;
        }

        public static bool IsEzDisabled
        {
            get => TttPlayer.Players.Count() < TttMapConfigs.MinEzPlayers;
        }

        public static bool IsHczDisabled
        {
            get => TttPlayer.Players.Count() < TttMapConfigs.MinHczPlayers;
        }

        public static IEnumerable<RoomName> AlwaysLocked
            => TttMapConfigs.AlwaysLocked;

        public static IEnumerable<RoomName> SpawnBlacklist
            => TttMapConfigs.SpawnBlacklist;

        public static bool IsLocked(FacilityZone zone)
            => lockingZones.Contains(zone) || lockedZones.Contains(zone);

        public static void Lock(FacilityZone zone)
        {
            if (lockedZones.Contains(zone) || lockingZones.Contains(zone))
                return;

            lockingZones.Add(zone);

            TttMapEvents.TriggerOnLockingZone(zone);

            foreach (var lightController in RoomLightController.Instances)
            {
                if (lightController is null || lightController.Room is null || lightController.Room.Zone != zone)
                    continue;

                lightController.NetworkOverrideColor = new Color(1f, 0f, 0f);
            }

            Timing.CallDelayed(20f, () =>
            {
                lockingZones.Remove(zone);
                lockedZones.Add(zone);

                foreach (var door in DoorVariant.AllDoors)
                {
                    if (door.Rooms is null || !door.Rooms.Any(room => room.Zone == zone))
                        continue;

                    door.Lock();
                    door.Close();
                    door.DisableInteracting();

                    lockedDoors.Add(door);
                }

                TttMapEvents.TriggerOnLockedZone(zone);
            });
        }

        public static TPickup Spawn<TPickup>(ItemType pickupType, Vector3 position, Vector3 scale, Quaternion rotation, bool spawn = true, Action<TPickup> setup = null)
            where TPickup : ItemPickupBase
        {
            if (!InventoryItemLoader.TryGetItem<ItemBase>(pickupType, out var item) || item?.PickupDropModel is null)
                return null;

            var pickup = UnityEngine.Object.Instantiate((TPickup)item.PickupDropModel);

            if (pickup is null)
                return null;

            pickup.transform.position = position;
            pickup.transform.localScale = scale;
            pickup.transform.rotation = rotation;

            pickup.NetworkInfo = new PickupSyncInfo(pickupType, item.Weight, ItemSerialGenerator.GenerateNext());
            pickup.Info.Locked = false;

            setup?.Invoke(pickup);

            if (spawn)
                NetworkServer.Spawn(pickup.gameObject);

            return pickup;
        }

        public static Vector3 GetSpawnPosition(bool removePos = true)
        {
            var validRooms = RoomIdentifier.AllRoomIdentifiers.Where(room => !SpawnBlacklist.Contains(room.Name) && !room.name.Contains("330") && ValidateRoom(room.Zone)).ToArray();
            var validRoom = validRooms.RandomItem();

            if (removePos)
            {
                while (usedSpawnPositions.Any(usedSpawnPos => Vector3.Distance(validRoom.transform.position, usedSpawnPos) <= 5f))
                {
                    validRoom = validRooms.RandomItem();
                }
            }

            var spawnPos = validRoom.transform.position;

            spawnPos.y += 1f;

            if (removePos)
                usedSpawnPositions.Add(spawnPos);

            return spawnPos;
        }

        private static bool ValidateRoom(FacilityZone zone)
        {
            if (lockingZones.Contains(zone) || lockedZones.Contains(zone))
                return false;

            if (zone is FacilityZone.Entrance && IsEzDisabled)
                return false;

            if (zone is FacilityZone.HeavyContainment && IsHczDisabled)
                return false;

            return zone != FacilityZone.None && zone != FacilityZone.Surface && zone != FacilityZone.Other;
        }

        private static void LockDoors()
        {
            lockedDoors.Clear();

            foreach (var room in RoomIdentifier.AllRoomIdentifiers)
            {
                if (room.ApiRoom is null || room.ApiRoom._doors is null || room.ApiRoom._doors.Count < 1)
                    continue;

                var door = room.ApiRoom._doors.Keys.OrderBy(d => d.DistanceSquared(room)).FirstOrDefault();

                if (door is null)
                    continue;

                if ((room.Name is RoomName.HczCheckpointToEntranceZone && IsEzDisabled)
                    || ((room.Name is RoomName.LczCheckpointA || room.Name is RoomName.LczCheckpointB) && IsHczDisabled)
                    || AlwaysLocked.Contains(room.Name) || room.name.Contains("330") || !ValidateRoom(room.Zone))
                {
                    door.Close();
                    door.Lock();
                    door.DisableInteracting();

                    lockedDoors.Add(door);

                    continue;
                }

                if (door is CheckpointDoor)
                {
                    door.Open();
                    door.Lock(DoorLockReason.DecontLockdown);
                    door.DisableInteracting();

                    lockedDoors.Add(door);

                    continue;
                }

                if (door is not ElevatorDoor)
                {
                    door.Open();
                    door.Lock();
                    door.DisableInteracting();

                    lockedDoors.Add(door);
                }
            }

            foreach (var door in DoorVariant.AllDoors)
            {
                if (lockedDoors.Contains(door))
                    continue;

                if (door.Rooms != null && door.Rooms.Any(r => r.Name is RoomName.Lcz914))
                {
                    door.Open();
                    door.Lock();
                    door.DisableInteracting();

                    lockedDoors.Add(door);

                    continue;
                }

                if (door.Rooms != null && door.Rooms.Any(r => r.Name.ToString().Contains("Checkpoint") || r.name.Contains("330")))
                    continue;

                if (door is BreakableDoor && door is not ElevatorDoor && door is not CheckpointDoor && door is not PryableDoor)
                {
                    door.Open();
                    door.Lock();
                    door.DisableInteracting();

                    lockedDoors.Add(door);

                    continue;
                }
            }
        }

        private static void DeleteStructures()
        {
            try
            {
                var structures = UnityEngine.Object.FindObjectsOfType<SpawnableStructure>();

                for (int i = 0; i < structures.Length; i++)
                    NetworkServer.Destroy(structures[i].gameObject);
            }
            catch { }
        }

        private static void DeleteStations()
        {
            try
            {
                foreach (var station in WorkstationController.AllWorkstations)
                    NetworkServer.Destroy(station.gameObject);
            }
            catch { }
        }

        private static void DeleteGates()
        {
            try
            {
                foreach (var tesla in TeslaGateController.Singleton.TeslaGates)
                    NetworkServer.Destroy(tesla.gameObject);

                TeslaGateController.Singleton.TeslaGates.Clear();
            }
            catch { }
        }

        private static void DeleteGlasses()
        {
            try
            {
                foreach (var window in UnityEngine.Object.FindObjectsOfType<BreakableWindow>())
                    NetworkServer.Destroy(window.gameObject);
            }
            catch { }
        }

        private static void ProgressLockdown()
        {
            if (!IsLocked(FacilityZone.Entrance) && !IsEzDisabled && Round.Duration.TotalMinutes >= 10)
                Lock(FacilityZone.Entrance);

            if (!IsLocked(FacilityZone.HeavyContainment) && !IsHczDisabled && Round.Duration.TotalMinutes >= (IsEzDisabled ? 10 : 20))
                Lock(FacilityZone.HeavyContainment);
        }

        [RoundStateChanged(Compendium.Enums.RoundState.InProgress)]
        private static void OnRoundStarted()
        {
            FLog.Info("Preparing the map ..");

            DeleteStructures();
            DeleteStations();
            DeleteGlasses();
            DeleteGates();

            LockDoors();

            DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
        }

        [RoundStateChanged(Compendium.Enums.RoundState.WaitingForPlayers)]
        private static void OnRoundWaiting()
        {
            lockedZones.Clear();
            lockingZones.Clear();
            lockedDoors.Clear();
            usedSpawnPositions.Clear();
        }
    }
}