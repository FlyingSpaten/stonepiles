using nrw.frese.stonepile.basics;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace nrw.frese.stonepile.item
{
    public class ItemPilableStone : ItemStone
    {
        public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            IWorldAccessor world = byEntity.World;
            BlockStonePile stonepileBlock = world.GetBlock(new AssetLocation("stonepiles:stonepile-" + Variant["rock"])) as BlockStonePile;

            ItemPilableUtil.HandleHeldInteractStart(itemslot, byEntity, blockSel, entitySel, firstEvent,ref handling, stonepileBlock, api, base.OnHeldInteractStart);
        }
    }
}
