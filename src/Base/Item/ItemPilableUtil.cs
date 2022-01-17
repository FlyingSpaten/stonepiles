
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace nrw.frese.stonepile.basics
{
    public class ItemPilableUtil
    {

        public delegate void DefaultHandle(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling);

        public static void HandleHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling, BlockPile blockPile, ICoreAPI api, DefaultHandle defaultHandle)
        {
            if (blockSel != null)
            {
                IPlayer byPlayer = (byEntity as EntityPlayer).Player;
                IWorldAccessor world = byEntity.World;
                BlockPos pos = blockSel.Position;
                Block selectedBlock = world.BlockAccessor.GetBlock(pos);

                if (blockPile != null && selectedBlock.BlockId != blockPile.BlockId)
                {
                    if (byEntity.Controls.Sprint)
                    {
                        if (blockPile == null) return;
                        BlockPos blockPos = pos.Copy();
                        if (byEntity.World.BlockAccessor.GetBlock(blockPos).Replaceable < 6000) blockPos.Add(blockSel.Face);

                        bool ok = blockPile.Construct(itemslot, byEntity.World, blockPos, byPlayer);

                        Cuboidf[] collisionBoxes = byEntity.World.BlockAccessor.GetBlock(blockPos).GetCollisionBoxes(byEntity.World.BlockAccessor, blockPos);

                        if (collisionBoxes != null && collisionBoxes.Length > 0 && CollisionTester.AabbIntersect(collisionBoxes[0], blockPos.X, blockPos.Y, blockPos.Z, byPlayer.Entity.CollisionBox, byPlayer.Entity.SidedPos.XYZ))
                        {
                            byPlayer.Entity.SidedPos.Y += collisionBoxes[0].Y2 - (byPlayer.Entity.SidedPos.Y - (int)byPlayer.Entity.SidedPos.Y);
                        }

                        if (ok)
                        {
                            handling = EnumHandHandling.PreventDefaultAction;
                            if (api.Side == EnumAppSide.Client)
                            {
                                ((byEntity as EntityPlayer).Player as IClientPlayer).TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
                            }
                        }
                        else
                        {
                            defaultHandle(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handling);
                        }

                        handling = EnumHandHandling.PreventDefault;
                        byEntity.Attributes.SetInt("aimingCancel", 1);
                        return;
                    }
                    else
                    {
                        defaultHandle(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handling);
                    }
                }
            }
            else
            {
                defaultHandle(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handling);
            }
        }
    }
}
