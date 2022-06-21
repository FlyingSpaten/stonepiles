
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace nrw.frese.stonepile.basics
{
    public abstract class BehaviorItemPilable : CollectibleBehavior
    {
        protected BehaviorItemPilable(CollectibleObject collObj) : base(collObj)
        {
        }
        public override void Initialize(JsonObject properties)
        {
            base.Initialize(properties);
        }

        public void DefaultHandle(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            itemslot.Itemstack.Item.OnHeldInteractStart(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        }

        public abstract BlockPile GetBlockPile(IWorldAccessor world, ItemSlot itemSlot);

        public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling, ref EnumHandling handling)
        {
            if (blockSel != null)
            {
                IPlayer byPlayer = (byEntity as EntityPlayer).Player;
                IWorldAccessor world = byEntity.World;
                BlockPos pos = blockSel.Position;
                Block selectedBlock = world.BlockAccessor.GetBlock(pos);
                BlockPile blockPile = GetBlockPile(world, itemslot);
                bool pileFull = pileIsFull(world, selectedBlock, blockPile, pos);

                if (blockPile != null && (selectedBlock.BlockId != blockPile.BlockId || pileFull))
                {
                    if (byEntity.Controls.Sprint || (pileFull && byEntity.Controls.Sneak))
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
                            handHandling = EnumHandHandling.PreventDefaultAction;
                            ICoreAPI api = world.Api;
                            if (api.Side == EnumAppSide.Client)
                            {
                                ((byEntity as EntityPlayer).Player as IClientPlayer).TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
                            }
                        }

                        handHandling = EnumHandHandling.PreventDefault;
                        byEntity.Attributes.SetInt("aimingCancel", 1);
                        return;
                    }
                }
            }
        }

        private bool pileIsFull(IWorldAccessor world, Block selectedBlock, BlockPile blockPile, BlockPos pos)
        {
            if(selectedBlock.BlockId == blockPile.BlockId)
            {
                BlockEntityPile blockEntityPile = (BlockEntityPile) world.BlockAccessor.GetBlockEntity(pos);
                return blockEntityPile.OwnStackSize() == blockEntityPile.MaxStackSize;
            }
            return false;
        }
    }
}
