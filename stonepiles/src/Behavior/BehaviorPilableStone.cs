
using nrw.frese.stonepile.basics;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace nrw.frese.stonepile.behavior
{
    public class BehaviorPilableStone : BehaviorItemPilable
    {
        public BehaviorPilableStone(CollectibleObject collObj) : base(collObj)
        {
        }

        public override BlockPile GetBlockPile(IWorldAccessor world, ItemSlot itemSlot)
        {
            RelaxedReadOnlyDictionary<string, string> Variant = collObj.Variant;
            return world.GetBlock(new AssetLocation("stonepiles:stonepile-" + Variant["rock"])) as BlockStonePile;
        }
    }
}
