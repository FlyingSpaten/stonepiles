using nrw.frese.stonepile.basics;
using nrw.frese.stonepile.block;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace nrw.frese.stonepile.item
{
    internal class ItemPilableFlint : ItemFlint
    {
        public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            IWorldAccessor world = byEntity.World;
            BlockFlintPile flintPileBlock = world.GetBlock(new AssetLocation("stonepiles:flintpile")) as BlockFlintPile;

            ItemPilableUtil.HandleHeldInteractStart(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handling, flintPileBlock, api, base.OnHeldInteractStart);
        }
    }
}
