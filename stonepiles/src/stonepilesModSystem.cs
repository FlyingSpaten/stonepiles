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
            api.RegisterBlockClass("BlockOreGradedPile", typeof(BlockOreGradedPile));
            api.RegisterBlockClass("BlockQuartzPile", typeof(BlockQuartzPile));
            api.RegisterBlockClass("BlockClayPile", typeof(BlockClayPile));
            api.RegisterBlockClass("BlockFlintPile", typeof(BlockFlintPile));
            api.RegisterBlockClass("BlockNuggetPile", typeof(BlockNuggetPile));

            api.RegisterBlockEntityClass("BlockEntityStonePile", typeof(BlockEntityStonePile));
            api.RegisterBlockEntityClass("BlockEntityOreUngradedPile", typeof(BlockEntityOreUngradedPile));
            api.RegisterBlockEntityClass("BlockEntityOreGradedPile", typeof(BlockEntityOreGradedPile));
            api.RegisterBlockEntityClass("BlockEntityQuartzPile", typeof(BlockEntityQuartzPile));
            api.RegisterBlockEntityClass("BlockEntityClayPile", typeof(BlockEntityClayPile));
            api.RegisterBlockEntityClass("BlockEntityFlintPile", typeof(BlockEntityFlintPile));
            api.RegisterBlockEntityClass("BlockEntityNuggetPile", typeof(BlockEntityNuggetPile));

            api.RegisterItemClass("ItemPilableStone", typeof(ItemPilableStone));
            api.RegisterItemClass("ItemPilableOreUngraded", typeof(ItemPilableOreUngraded));
            api.RegisterItemClass("ItemPilableOreGraded", typeof(ItemPilableOreGraded));
            api.RegisterItemClass("ItemPilableQuartz", typeof(ItemPilableQuartz));
            api.RegisterItemClass("ItemPilableClay", typeof(ItemPilableClay));
            api.RegisterItemClass("ItemPilableFlint", typeof(ItemPilableFlint));
            api.RegisterItemClass("ItemPilableNugget", typeof(ItemPilableNugget));

            api.RegisterCollectibleBehaviorClass("BehaviorPilableQuartz", typeof(BehaviorPilableQuartz));
            api.RegisterCollectibleBehaviorClass("BehaviorPilableStone", typeof(BehaviorPilableStone));

            api.Logger.Warning("Stonepiles loaded");
        }
    }
}
