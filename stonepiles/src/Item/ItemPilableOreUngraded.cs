using System;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace nrw.frese.stonepile.item
{
    public class ItemPilableOreUngraded : ItemOre
    {
        public override bool IsPileable { get{ return true; } }
        protected override AssetLocation PileBlockCode { get {
                if (IsCoal)
                {
                    return new AssetLocation("coalpile");
                }
                return new AssetLocation("stonepiles:orepile-ungraded-" + Variant["ore"]);
            } }

    }
}
