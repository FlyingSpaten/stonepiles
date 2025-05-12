using nrw.frese.stonepile.basics;
using nrw.frese.stonepile.block;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace nrw.frese.stonepile.item
{
    internal class ItemPilableNugget : ItemNugget
    {
        public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            IWorldAccessor world = byEntity.World;
            BlockNuggetPile nuggetPileBlock = world.GetBlock(new AssetLocation("stonepiles:nuggetpile-" + Variant["ore"])) as BlockNuggetPile;

            ItemPilableUtil.HandleHeldInteractStart(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handling, nuggetPileBlock, api, base.OnHeldInteractStart);
        }
    }
}
