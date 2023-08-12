using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace nrw.frese.stonepile.basics
{
    public abstract class BlockPile : Block, IBlockItemPile
    {
        Cuboidf[][] CollisionBoxesByFillLevel;

        public abstract String AddStoneLabel { get; }
        public abstract String RemoveStoneLabel { get; }

        public abstract int DefaultAddQuantity { get; }

        public BlockPile()
        {
            CollisionBoxesByFillLevel = new Cuboidf[9][];

            for (int i = 0; i <= 8; i++)
            {
                CollisionBoxesByFillLevel[i] = new Cuboidf[] { new Cuboidf(0, 0, 0, 1, i * 0.125f, 1) };
            }
        }

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            if (api.Side != EnumAppSide.Client) return;
            ICoreClientAPI capi = api as ICoreClientAPI;
        }

        public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
        {
            BlockEntityPile bea = world.BlockAccessor.GetBlockEntity(selection.Position) as BlockEntityPile;
            if (bea != null && bea.inventory[0].Itemstack != null)
            {
                return new WorldInteraction[] {
                    new WorldInteraction()
                    {
                        ActionLangCode = AddStoneLabel,
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = "sneak",
                        Itemstacks = new ItemStack[] { new ItemStack(bea.inventory[0].Itemstack.Item, DefaultAddQuantity) }
                    },
                    new WorldInteraction()
                    {
                        ActionLangCode = RemoveStoneLabel,
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = null
                    }
                };
            }
            return new WorldInteraction[] { };
        }

        public override void GetDecal(IWorldAccessor world, BlockPos pos, ITexPositionSource decalTexSource, ref MeshData decalModelData, ref MeshData blockModelData)
        {
            BlockEntityPile blockEntityPile = world.BlockAccessor.GetBlockEntity(pos) as BlockEntityPile;
            if (blockEntityPile == null)
            {
                base.GetDecal(world, pos, decalTexSource, ref decalModelData, ref blockModelData);
                return;
            }

            decalModelData.Clear();
            blockEntityPile.GetDecalMesh(decalTexSource, out decalModelData);
        }

        public bool Construct(ItemSlot slot, IWorldAccessor world, BlockPos pos, IPlayer player)
        {
            Block block = world.BlockAccessor.GetBlock(pos);
            if (!block.IsReplacableBy(this)) return false;
            Block belowBlock = world.BlockAccessor.GetBlock(pos.DownCopy());
            if (!belowBlock.CanAttachBlockAt(world.BlockAccessor, this, pos.DownCopy(), BlockFacing.UP) /*&& (belowBlock != this || FillLevel(world.BlockAccessor, pos.DownCopy()) != 4)*/) return false;

            world.BlockAccessor.SetBlock(BlockId, pos);

            BlockEntity be = world.BlockAccessor.GetBlockEntity(pos);
            if (be is BlockEntityPile)
            {
                BlockEntityPile pile = (BlockEntityPile)be;
                if (player == null || player.WorldData.CurrentGameMode != EnumGameMode.Creative)
                {
                    pile.inventory[0].Itemstack = (ItemStack)slot.TakeOut(player.Entity.Controls.Sprint == true ? pile.BulkTakeQuantity : pile.DefaultTakeQuantity);
                    slot.MarkDirty();
                }
                else
                {
                    pile.inventory[0].Itemstack = (ItemStack)slot.Itemstack.Clone();
                    pile.inventory[0].Itemstack.StackSize = Math.Min(pile.inventory[0].Itemstack.StackSize, pile.MaxStackSize);
                }

                pile.MarkDirty();
                world.BlockAccessor.MarkBlockDirty(pos);
                //world.PlaySoundAt(pile.SoundLocation, pos.X, pos.Y, pos.Z, player, true);
            }

            return true;
        }

        public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
        {
            BlockEntityPile bea = world.BlockAccessor.GetBlockEntity(pos) as BlockEntityPile;
            if (bea != null && bea.inventory[0].Itemstack != null) return bea.inventory[0].Itemstack.Clone();

            return base.OnPickBlock(world, pos);
        }

        public int GetLayercount(IWorldAccessor world, BlockPos pos)
        {
            BlockEntityPile bea = world.BlockAccessor.GetBlockEntity(pos) as BlockEntityPile;
            if (bea != null) return bea.Layers();
            return 0;
        }

        public override Cuboidf[] GetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
        {
            BlockEntityPile bea = blockAccessor.GetBlockEntity(pos) as BlockEntityPile;
            if (bea == null) return CollisionBoxesByFillLevel[1];

            return CollisionBoxesByFillLevel[bea.Layers()];
        }

        public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
        {
            BlockEntityPile bea = blockAccessor.GetBlockEntity(pos) as BlockEntityPile;
            if (bea == null || bea.Layers() <= 0) return CollisionBoxesByFillLevel[1];

            return CollisionBoxesByFillLevel[bea.Layers()];
        }


        public override BlockDropItemStack[] GetDropsForHandbook(ItemStack handbookStack, IPlayer forPlayer)
        {
            return new BlockDropItemStack[0];
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
        {
            // Handled by BlockEntityItemPile
            return new ItemStack[0];
        }

        public override bool OnFallOnto(IWorldAccessor world, BlockPos pos, Block block, TreeAttribute blockEntityAttributes)
        {
            if (block is BlockPile)
            {
                BlockEntityPile be = world.BlockAccessor.GetBlockEntity(pos) as BlockEntityPile;
                if (be != null)
                {
                    return be.MergeWith(blockEntityAttributes);
                }
            }

            return base.OnFallOnto(world, pos, block, blockEntityAttributes);
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            BlockEntity be = world.BlockAccessor.GetBlockEntity(blockSel.Position);
            if (be is BlockEntityPile)
            {
                BlockEntityPile pile = (BlockEntityPile)be;
                return pile.OnPlayerInteract(byPlayer);
            }

            return false;
        }

        public override void OnNeighbourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos)
        {
            Block belowBlock = world.BlockAccessor.GetBlock(pos.DownCopy());
            if (!belowBlock.CanAttachBlockAt(world.BlockAccessor, this, pos.DownCopy(), BlockFacing.UP) /*&& (belowBlock != this || FillLevel(world.BlockAccessor, pos.DownCopy()) < 4)*/)
            {
                world.BlockAccessor.BreakBlock(pos, null);
            }
        }


        public override bool CanAttachBlockAt(IBlockAccessor blockAccessor, Block block, BlockPos pos, BlockFacing blockFace, Cuboidi attachmentArea = null)
        {
            BlockEntityPile be = blockAccessor.GetBlockEntity(pos) as BlockEntityPile;
            if (be != null)
            {
                return be.OwnStackSize() == be.MaxStackSize;
            }

            return base.CanAttachBlockAt(blockAccessor, block, pos, blockFace, attachmentArea);
        }

    }
}
