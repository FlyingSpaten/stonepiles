using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace nrw.frese.stonepile.item
{
    internal class ItemPilableQuartz : ItemOre
    {
        public override bool IsPileable { get { return true; } }
        protected override AssetLocation PileBlockCode
        {
            get
            {
                return new AssetLocation("stonepiles:quartzpile-" + Code.Path);
            }
        }

    }
}
