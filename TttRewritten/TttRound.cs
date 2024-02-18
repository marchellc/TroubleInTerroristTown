using Compendium.Updating;

using GameCore;

using helpers;

using MEC;

using RoundRestarting;
using System.Linq;

using TttRewritten.Configs;
using TttRewritten.Enums;
using TttRewritten.Events.Custom;

using Compendium.Attributes;
using Compendium.Features;

using PluginAPI.Core;

namespace TttRewritten
{
    public static class TttRound
    {
        private static TttRoundStatus roundStatus = TttRoundStatus.None;
        private static TttRoundStatus prevStatus = TttRoundStatus.None;

        private static TttRoleType winningRole = TttRoleType.None;

        public static bool IsLocked { get; set; }
        public static bool IsForced { get; set; }

        public static bool IsInProgress
        {
            get => Status is TttRoundStatus.Running;
        }

        public static bool IsEnding
        {
            get => Status is TttRoundStatus.Ending;
        }

        public static bool IsEnded
        {
            get => Status is TttRoundStatus.Ended;
        }

        public static bool IsWaiting
        {
            get => Status is TttRoundStatus.Waiting;
        }

        public static bool IsLobbyLocked
        {
            get => RoundStart.LobbyLock;
            set => RoundStart.LobbyLock = value;
        }

        public static bool IsRoundLocked
        {
            get => RoundSummary.RoundLock;
            set => RoundSummary.RoundLock = value;
        }

        public static float RestartDelay
        {
            get => TttRoundConfigs.RestartDelay <= 1f ? 1f : TttRoundConfigs.RestartDelay;
        }

        public static float EndTime
        {
            get
            {
                if (TttMap.IsEzDisabled)
                    return 20f;

                if (TttMap.IsHczDisabled)
                    return 15f;

                return 30f;
            }
        }

        public static TttRoundStatus Status
        {
            get => roundStatus;
            set => roundStatus = value;
        }

        public static TttRoleType Winner
        {
            get => winningRole;
            set
            {
                winningRole = value;
                roundStatus = TttRoundStatus.Ending;
            }
        }

        [Update(IsUnity = true, PauseRestarting = false, PauseWaiting = false, Delay = 50)]
        internal static void UpdateRound()
        {
            if (prevStatus != roundStatus)
            {
                FLog.Info($"Round Status change detected ({prevStatus} -> {roundStatus})");

                switch (roundStatus)
                {
                    case TttRoundStatus.Waiting:
                        IsLobbyLocked = true;
                        TttRoundEvents.TriggerOnWaiting();
                        roundStatus = TttRoundStatus.Waiting;
                        break;

                    case TttRoundStatus.Starting:
                        IsLobbyLocked = false;
                        TttRoundEvents.TriggerOnStarting();
                        roundStatus = TttRoundStatus.Starting;
                        break;

                    case TttRoundStatus.Running:
                        IsRoundLocked = true;
                        IsForced = false;
                        TttRoundEvents.TriggerOnStarted(TttPlayer.Players);
                        roundStatus = TttRoundStatus.Running;
                        break;

                    case TttRoundStatus.Ending:
                        TttRoundEvents.TriggerOnEnding(RestartDelay, winningRole);
                        roundStatus = TttRoundStatus.Ending;
                        Timing.CallDelayed(2f, () => roundStatus = TttRoundStatus.Ended);
                        break;

                    case TttRoundStatus.Ended:
                        TttRoundEvents.TriggerOnEnded(winningRole);
                        Timing.CallDelayed(RestartDelay, RoundRestart.InitiateRoundRestart);
                        roundStatus = TttRoundStatus.Ended;
                        break;
                }

                prevStatus = roundStatus;
                return;
            }

            if (roundStatus != TttRoundStatus.Running)
            {
                if (roundStatus is TttRoundStatus.Starting)
                {
                    if (TttPlayer.ValidPlayers.Count() < TttRoundConfigs.MinPlayers && !IsForced)
                        roundStatus = TttRoundStatus.Waiting;
                }
                else
                {
                    if (roundStatus is TttRoundStatus.Waiting
                        && (TttPlayer.ValidPlayers.Count() >= TttRoundConfigs.MinPlayers || IsForced))
                        roundStatus = TttRoundStatus.Starting;
                }
            }
            else
            {
                TttRoundEvents.TriggerOnUpdate();

                if (IsLocked)
                {
                    roundStatus = TttRoundStatus.Running;
                    winningRole = TttRoleType.None;
                    return;
                }

                if (Round.Duration.TotalMinutes >= EndTime)
                {
                    roundStatus = TttRoundStatus.Ending;
                    winningRole = TttRoleType.Dead;
                    return;
                }

                var detectiveCount = TttPlayer.Players.Count(pl => pl.RoleType is TttRoleType.Detective);
                var murdererCount = TttPlayer.Players.Count(pl => pl.RoleType is TttRoleType.Murderer);
                var innocentCount = TttPlayer.Players.Count(pl => pl.RoleType is TttRoleType.Innocent);
                var deadCount = TttPlayer.Players.Count(pl => pl.RoleType is TttRoleType.Dead);
                var totalCount = TttPlayer.Players.Count();

                if (deadCount == totalCount)
                {
                    winningRole = TttRoleType.Dead;
                    roundStatus = TttRoundStatus.Ending;
                    return;
                }

                if (detectiveCount > 0 && murdererCount < 1)
                {
                    winningRole = TttRoleType.Detective;
                    roundStatus = TttRoundStatus.Ending;
                    return;
                }

                if (innocentCount > 0 && murdererCount < 1 && detectiveCount < 1)
                {
                    winningRole = TttRoleType.Innocent;
                    roundStatus = TttRoundStatus.Ending;
                    return;
                }

                if (murdererCount > 0 && detectiveCount < 1 && innocentCount < 1)
                {
                    winningRole = TttRoleType.Murderer;
                    roundStatus = TttRoundStatus.Ending;
                    return;
                }

                if (murdererCount == 0 && detectiveCount == 0 && innocentCount == 0)
                {
                    winningRole = TttRoleType.Dead;
                    roundStatus = TttRoundStatus.Ending;
                    return;
                }
            }
        }

        [RoundStateChanged(Compendium.Enums.RoundState.WaitingForPlayers)]
        private static void OnWaiting()
            => roundStatus = TttRoundStatus.Waiting;

        [RoundStateChanged(Compendium.Enums.RoundState.InProgress)]
        private static void OnStarted()
            => roundStatus = TttRoundStatus.Running;
    }
}