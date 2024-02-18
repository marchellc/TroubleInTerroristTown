using System;
using System.Collections.Generic;

using TttRewritten.Enums;

namespace TttRewritten.Events.Custom
{
    public static class TttRoundEvents
    {
        public static event Action OnWaiting;
        public static event Action OnRestarting;
        public static event Action OnStarting;
        public static event Action OnUpdate;

        public static event Action<IEnumerable<TttPlayer>> OnStarted;
        public static event Action<float, TttRoleType> OnEnding;

        public static event Action<TttRoleType> OnEnded;

        internal static void TriggerOnWaiting()
            => OnWaiting?.Invoke();

        internal static void TriggerOnRestarting()
            => OnRestarting?.Invoke();

        internal static void TriggerOnUpdate()
            => OnUpdate?.Invoke();

        internal static void TriggerOnEnded(TttRoleType winner)
            => OnEnded?.Invoke(winner);

        internal static void TriggerOnEnding(float delay, TttRoleType winner)
            => OnEnding?.Invoke(delay, winner);

        internal static void TriggerOnStarted(IEnumerable<TttPlayer> players)
            => OnStarted?.Invoke(players);

        internal static void TriggerOnStarting()
            => OnStarting?.Invoke();
    }
}