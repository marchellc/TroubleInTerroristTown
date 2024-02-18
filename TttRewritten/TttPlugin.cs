using Compendium.Features;

namespace TttRewritten
{
    public class TttPlugin : ConfigFeatureBase
    {
        public override bool IsPatch => true;
        public override bool CanBeShared => true;

        public override string Name => "Trouble In Terrorist Town";
    }
}