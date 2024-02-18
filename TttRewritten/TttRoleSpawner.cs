using Compendium.Attributes;
using Compendium.Features;

using helpers.Attributes;

using System.Collections.Generic;
using System.Linq;

using TttRewritten.Enums;
using TttRewritten.Events.Custom;

namespace TttRewritten
{
    public static class TttRoleSpawner
    {
        private static bool rolesAssigned;

        public static TttRoleType[] Roles;
        public static TttPlayer[] Players;

        public static string PreviousMurdererId;
        public static string PreviousDetectiveId;

        [Load]
        internal static void Init()
        {
            TttRoundEvents.OnStarted += OnRoundStarted;
        }

        public static void GenerateRoles()
        {
            Roles = new TttRoleType[Players.Length];

            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].IsOverwatchEnabled)
                {
                    Roles[i] = TttRoleType.Dead;
                    continue;
                }

                Roles[i] = TttRoleType.Innocent;
            }

            var possibleDetectiveIndexes = new List<int>();
            var possibleMurdererIndexes = new List<int>();

            for (int i = 0; i < Players.Length; i++)
            {
                if (CanBeDetective(Players[i]))
                    possibleDetectiveIndexes.Add(i);

                if (CanBeMurderer(Players[i]))
                    possibleMurdererIndexes.Add(i);
            }

            var detectiveCount = 3;
            var murdererCount = 4;

            if (TttMap.IsEzDisabled)
            {
                detectiveCount -= 1;
                murdererCount -= 2;
            }

            if (TttMap.IsHczDisabled)
            {
                detectiveCount -= 1;
                murdererCount -= 1;
            }

            for (int i = 0; i < detectiveCount; i++)
            {
                if (possibleDetectiveIndexes.Count > 0)
                {
                    var detectiveIndex = possibleDetectiveIndexes.RandomItem();

                    Roles[detectiveIndex] = TttRoleType.Detective;
                    PreviousDetectiveId = Players[detectiveIndex].UserId;

                    possibleMurdererIndexes.Remove(detectiveIndex);
                    possibleDetectiveIndexes.Remove(detectiveIndex);

                    FLog.Info($"Selected detective: {Players[detectiveIndex].Nickname}");
                }
            }

            for (int i = 0; i < murdererCount; i++)
            {
                if (possibleMurdererIndexes.Count > 0)
                {
                    var murdererIndex = possibleMurdererIndexes.RandomItem();

                    Roles[murdererIndex] = TttRoleType.Murderer;
                    PreviousMurdererId = Players[murdererIndex].UserId;

                    possibleMurdererIndexes.Remove(murdererIndex);

                    FLog.Info($"Selected murderer: {Players[murdererIndex].Nickname}");
                }
            }
        }

        private static void AssignRoles()
        {
            if (rolesAssigned)
                return;

            rolesAssigned = true;

            GenerateRoles();

            for (int i = 0; i < Roles.Length; i++)
                Players[i].RoleManager.Set(Roles[i], TttSpawnReason.RoundStart);

            TttRoleEvents.TriggerOnFinishedAssigningRoles();
        }

        private static bool CanBeDetective(TttPlayer player)
            => !player.IsOverwatchEnabled && (PreviousDetectiveId is null || PreviousDetectiveId != player.UserId);

        private static bool CanBeMurderer(TttPlayer player)
            => !player.IsOverwatchEnabled && (PreviousMurdererId is null || PreviousMurdererId != player.UserId);

        private static void OnRoundStarted(IEnumerable<TttPlayer> players)
        {
            Players = players.ToArray();
            AssignRoles();
        }

        [RoundStateChanged(Compendium.Enums.RoundState.WaitingForPlayers)]
        private static void OnWaiting()
            => rolesAssigned = false;
    }
}