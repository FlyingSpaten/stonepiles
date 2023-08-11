
using nrw.frese.stonepile.basics;
using nrw.frese.stonepile.block;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace nrw.frese.stonepile.behavior
{
    public class BehaviorPilableQuartz : BehaviorItemPilable
    {
        public BehaviorPilableQuartz(CollectibleObject collObj) : base(collObj)
        {
        }

        public override BlockPile GetBlockPile(IWorldAccessor world, ItemSlot itemSlot)
        {
            RelaxedReadOnlyDictionary<string, string> Variant = itemSlot.Itemstack.Item.Variant;
            return world.GetBlock(new AssetLocation("stonepiles:quartzpile-" + Variant["code"])) as BlockQuartzPile;
        }
    }
}