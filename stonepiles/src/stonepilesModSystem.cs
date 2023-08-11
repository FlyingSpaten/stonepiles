using nrw.frese.stonepile.behavior;
using nrw.frese.stonepile.block;
using nrw.frese.stonepile.blockentity;
using nrw.frese.stonepile.item;
using Vintagestory.API.Common;

namespace nrw.frese.stonepile
{
    public class stonepilesModSystem : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            api.RegisterBlockClass("BlockStonePile", typeof(BlockStonePile));
            api.RegisterBlockClass("BlockOreUngradedPile", typeof(BlockOreUngradedPile));
            api.RegisterBlockClass("BlockQuartzPile", typeof(BlockQuartzPile));
            api.RegisterBlockClass("BlockClayPile", typeof(BlockClayPile));

            api.RegisterBlockEntityClass("BlockEntityStonePile", typeof(BlockEntityStonePile));
            api.RegisterBlockEntityClass("BlockEntityOreUngradedPile", typeof(BlockEntityOreUngradedPile));
            api.RegisterBlockEntityClass("BlockEntityQuartzPile", typeof(BlockEntityQuartzPile));
            api.RegisterBlockEntityClass("BlockEntityClayPile", typeof(BlockEntityClayPile));

            api.RegisterItemClass("ItemPilableStone", typeof(ItemPilableStone));
            api.RegisterItemClass("ItemPilableOre", typeof(ItemPilableOre));
            api.RegisterItemClass("ItemPilableQuartz", typeof(ItemPilableQuartz));
            api.RegisterItemClass("ItemPilableClay", typeof(ItemPilableClay));

            api.RegisterCollectibleBehaviorClass("BehaviorPilableQuartz", typeof(BehaviorPilableQuartz));
            api.RegisterCollectibleBehaviorClass("BehaviorPilableStone", typeof(BehaviorPilableStone));
        }
    }
}
