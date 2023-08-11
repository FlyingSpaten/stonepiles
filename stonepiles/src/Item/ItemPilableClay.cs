using nrw.frese.stonepile.basics;
using nrw.frese.stonepile.block;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace nrw.frese.stonepile.item
{
    internal class ItemPilableClay : ItemClay
    {
        public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            IWorldAccessor world = byEntity.World;
            BlockClayPile stonepileBlock = world.GetBlock(new AssetLocation("stonepiles:claypile-" + Variant["type"])) as BlockClayPile;

            ItemPilableUtil.HandleHeldInteractStart(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handling, stonepileBlock, api, base.OnHeldInteractStart);
        }
    }
}
