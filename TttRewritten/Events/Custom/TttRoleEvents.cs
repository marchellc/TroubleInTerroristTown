using System;

namespace TttRewritten.Events.Custom
{
    public static class TttRoleEvents
    {
        public static event Action OnFinishedAssigningRoles;

        internal static void TriggerOnFinishedAssigningRoles()
            => OnFinishedAssigningRoles?.Invoke();
    }
}