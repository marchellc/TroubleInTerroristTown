using MapGeneration;

using System;

namespace TttRewritten.Events.Custom
{
    public static class TttMapEvents
    {
        public static event Action<FacilityZone> OnLockingZone;
        public static event Action<FacilityZone> OnLockedZone;

        internal static void TriggerOnLockingZone(FacilityZone facilityZone)
            => OnLockingZone?.Invoke(facilityZone);

        internal static void TriggerOnLockedZone(FacilityZone facilityZone)
            => OnLockedZone?.Invoke(facilityZone);
    }
}