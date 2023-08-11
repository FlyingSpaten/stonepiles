using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace nrw.frese.stonepile.basics
{
    public abstract class BlockEntityPile : BlockEntity, ITexPositionSource
    {

        public InventoryGeneric inventory;
        public object inventoryLock = new object();

        public ICoreClientAPI capi;

        public abstract string BlockCode { get; }
        public abstract int MaxStackSize { get; }
        public abstract int DefaultTakeQuantity { get; }
        public abstract int BulkTakeQuantity { get; }
        public abstract AssetLocation SoundLocation { get; }

        public BlockEntityPile()
        {
            inventory = new InventoryGeneric(1, BlockCode, null, null, null);
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);

            capi = api as ICoreClientAPI;

            inventory.LateInitialize(BlockCode + "-" + Pos.ToString(), api);
            inventory.ResolveBlocksOrItems();
        }

        public Size2i AtlasSize
        {
            get { return ((ICoreClientAPI)Api).BlockTextureAtlas.Size; }
        }

        public TextureAtlasPosition this[string textureCode]
        {
            get
            {
                return capi.BlockTextureAtlas.Positions[Block.Textures[textureCode].Baked.TextureSubId];
            }
        }

        public int Layers()
        {
            return Math.Max(Math.Min(inventory[0].StackSize / (MaxStackSize / 8), 8), 1);
        }

        public void GetDecalMesh(ITexPositionSource decalTexSource, out MeshData meshdata)
        {
            int size = Layers() * 2;

            Shape shape = capi.TesselatorManager.GetCachedShape(new AssetLocation("block/basic/layers/" + GameMath.Clamp(size, 2, 16) + "voxel"));
            capi.Tesselator.TesselateShape(BlockCode, shape, out meshdata, decalTexSource);
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            lock (inventoryLock)
            {
                if (!inventory[0].Empty)
                {
                    int size = Layers() * 2;

                    Shape shape = capi.TesselatorManager.GetCachedShape(new AssetLocation("block/basic/layers/" + GameMath.Clamp(size, 2, 16) + "voxel"));
                    MeshData meshdata;
                    capi.Tesselator.TesselateShape(BlockCode, shape, out meshdata, this);

                    mesher.AddMeshData(meshdata);
                }
            }

            return true;
        }

        public override void OnBlockBroken(IPlayer byPlayer = null)
        {
            if (Api.World is IServerWorldAccessor)
            {
                ItemSlot slot = inventory[0];
                while (slot.StackSize > 0)
                {
                    ItemStack split = slot.TakeOut(GameMath.Clamp(slot.StackSize, 1, System.Math.Max(1, slot.Itemstack.Collectible.MaxStackSize / 2)));
                    Api.World.SpawnItemEntity(split, Pos.ToVec3d().Add(0.5, 0.5, 0.5));
                }
            }
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            if (tree.GetTreeAttribute("inventory") != null)
            {
                inventory.FromTreeAttributes(tree.GetTreeAttribute("inventory"));
                if (Api != null)
                {
                    inventory.Api = Api;
                    inventory.ResolveBlocksOrItems();
                }
            }
            else
            {
                inventory = new InventoryGeneric(1, BlockCode, null, null, null);
            }

            if (Api is ICoreClientAPI)
            {
                Api.World.BlockAccessor.MarkBlockDirty(Pos);
            }
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            ITreeAttribute invtree = new TreeAttribute();
            inventory.ToTreeAttributes(invtree);
            tree["inventory"] = invtree;
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            ItemStack stack = inventory[0].Itemstack;
            if (stack == null) return;

            dsc.AppendLine(stack.StackSize + "x " + stack.GetName());
        }

        public int OwnStackSize()
        {
            return inventory[0] != null ? inventory[0].StackSize : 0;
        }

        public bool OnPlayerInteract(IPlayer byPlayer)
        {
            bool ok = baseOnPlayerInterace(byPlayer);

            TriggerPileChanged();

            return ok;
        }

        private bool baseOnPlayerInterace(IPlayer byPlayer)
        {
            BlockPos abovePos = Pos.UpCopy();
            BlockEntity be = Api.World.BlockAccessor.GetBlockEntity(abovePos);
            if (be is BlockEntityPile)
            {
                return ((BlockEntityPile)be).OnPlayerInteract(byPlayer);
            }

            bool sneaking = byPlayer.Entity.Controls.Sneak;


            ItemSlot hotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;

            bool equalStack = hotbarSlot.Itemstack != null && hotbarSlot.Itemstack.Equals(Api.World, inventory[0].Itemstack, GlobalConstants.IgnoredStackAttributes);

            if (sneaking && !equalStack)
            {
                return false;
            }

            if (sneaking && equalStack && OwnStackSize() >= MaxStackSize)
            {
                Block pileblock = Api.World.BlockAccessor.GetBlock(Pos);
                Block aboveblock = Api.World.BlockAccessor.GetBlock(abovePos);

                if (aboveblock.IsReplacableBy(pileblock))
                {
                    if (Api.World is IServerWorldAccessor)
                    {
                        Api.World.BlockAccessor.SetBlock(pileblock.Id, abovePos);
                        BlockEntityPile bep = Api.World.BlockAccessor.GetBlockEntity(abovePos) as BlockEntityPile;
                        if (bep != null) bep.TryPutItem(byPlayer);
                    }
                    return true;
                }

                return false;
            }

            lock (inventoryLock)
            {
                if (sneaking)
                {
                    return TryPutItem(byPlayer);
                }
                else
                {
                    return TryTakeItem(byPlayer);
                }
            }
        }


        void TriggerPileChanged()
        {
            if (Api.Side != EnumAppSide.Server) return;

            int maxSteepness = 4;

            BlockPile belowstonepile = Api.World.BlockAccessor.GetBlock(Pos.DownCopy()) as BlockPile;
            int belowwlayers = belowstonepile == null ? 0 : belowstonepile.GetLayercount(Api.World, Pos.DownCopy());

            foreach (var face in BlockFacing.HORIZONTALS)
            {
                BlockPos npos = Pos.AddCopy(face);
                Block nblock = Api.World.BlockAccessor.GetBlock(npos);
                BlockPile nblockstonepile = Api.World.BlockAccessor.GetBlock(npos) as BlockPile;
                int nblockstonepilelayers = nblockstonepile == null ? 0 : nblockstonepile.GetLayercount(Api.World, npos);

                // When should it collapse?
                // When there layers > 3 and nearby is air or replacable
                // When nearby is stone and herelayers - neiblayers > 3
                // When there is stone below us, the neighbour below us is stone, nearby is air or replaceable, and ownstone+belowstone - neibbelowstone > 3

                int layerdiff = Math.Max(nblock.Replaceable > 6000 ? Math.Max(0, Layers() - maxSteepness) : 0, (nblockstonepile != null ? Layers() - nblockstonepilelayers - maxSteepness : 0));

                if (belowwlayers > 0)
                {
                    BlockPile nbelowblockstonepile = Api.World.BlockAccessor.GetBlock(npos.DownCopy()) as BlockPile;
                    int nbelowwlayers = nbelowblockstonepile == null ? 0 : nbelowblockstonepile.GetLayercount(Api.World, npos.DownCopy());
                    layerdiff = Math.Max(layerdiff, (nbelowblockstonepile != null ? Layers() + belowwlayers - nbelowwlayers - maxSteepness : 0));
                }

                if (Api.World.Rand.NextDouble() < layerdiff / (float)maxSteepness)
                {
                    if (TryPartialCollapse(npos.UpCopy(), MaxStackSize / 8)) return;
                }
            }
        }

        public bool MergeWith(TreeAttribute blockEntityAttributes)
        {
            InventoryGeneric otherinv = new InventoryGeneric(1, BlockCode, null, null, null);
            otherinv.FromTreeAttributes(blockEntityAttributes.GetTreeAttribute("inventory"));
            otherinv.Api = Api;
            otherinv.ResolveBlocksOrItems();

            if (!inventory[0].Empty && otherinv[0].Itemstack.Equals(Api.World, inventory[0].Itemstack, GlobalConstants.IgnoredStackAttributes))
            {
                int quantityToMove = Math.Min(otherinv[0].StackSize, Math.Max(0, MaxStackSize - inventory[0].StackSize));
                inventory[0].Itemstack.StackSize += quantityToMove;

                otherinv[0].TakeOut(quantityToMove);
                if (otherinv[0].StackSize > 0)
                {
                    BlockPos uppos = Pos.UpCopy();
                    Block upblock = Api.World.BlockAccessor.GetBlock(uppos);
                    if (upblock.Replaceable > 6000)
                    {
                        ((BlockPile)Block).Construct(otherinv[0], Api.World, uppos, null);
                    }
                }

                MarkDirty(true);
                TriggerPileChanged();
            }

            return true;
        }


        private bool TryPartialCollapse(BlockPos pos, int quantity)
        {
            if (inventory[0].Empty) return false;

            IWorldAccessor world = Api.World;

            if (world.Side == EnumAppSide.Server)
            {
                ICoreServerAPI sapi = (world as IServerWorldAccessor).Api as ICoreServerAPI;
                if (!sapi.Server.Config.AllowFallingBlocks) return false;
            }

            if (IsReplacableBeneath(world, pos) || IsReplacableBeneathAndSideways(world, pos))
            {
                // Prevents duplication
                Entity entity = world.GetNearestEntity(pos.ToVec3d().Add(0.5, 0.5, 0.5), 1, 1.5f, (e) =>
                {
                    return e is EntityBlockFalling && ((EntityBlockFalling)e).initialPos.Equals(pos);

                });

                if (entity == null)
                {
                    int prevstacksize = inventory[0].StackSize;

                    inventory[0].Itemstack.StackSize = quantity;
                    EntityBlockFalling entityblock = new EntityBlockFalling(Block, this, pos, null, 1, true, 0.05f);
                    entityblock.DoRemoveBlock = false; // We want to split the pile, not remove it 
                    world.SpawnEntity(entityblock);
                    entityblock.ServerPos.Y -= 0.25f;
                    entityblock.Pos.Y -= 0.25f;

                    inventory[0].Itemstack.StackSize = prevstacksize - quantity;
                    return true;
                }
            }


            return false;
        }


        private bool IsReplacableBeneathAndSideways(IWorldAccessor world, BlockPos pos)
        {
            for (int i = 0; i < 4; i++)
            {
                BlockFacing facing = BlockFacing.HORIZONTALS[i];

                Block nBlock = world.BlockAccessor.GetBlockOrNull(pos.X + facing.Normali.X, pos.Y + facing.Normali.Y, pos.Z + facing.Normali.Z);
                Block nBBlock = world.BlockAccessor.GetBlockOrNull(pos.X + facing.Normali.X, pos.Y + facing.Normali.Y - 1, pos.Z + facing.Normali.Z);

                if (nBlock != null && nBBlock != null && nBlock.Replaceable >= 6000 && nBBlock.Replaceable >= 6000)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsReplacableBeneath(IWorldAccessor world, BlockPos pos)
        {
            Block bottomBlock = world.BlockAccessor.GetBlock(pos.X, pos.Y - 1, pos.Z);
            return (bottomBlock != null && bottomBlock.Replaceable > 6000);
        }

        public virtual bool TryPutItem(IPlayer player)
        {
            if (OwnStackSize() >= MaxStackSize) return false;

            ItemSlot hotbarSlot = player.InventoryManager.ActiveHotbarSlot;

            if (hotbarSlot.Itemstack == null) return false;

            ItemSlot invSlot = inventory[0];

            if (invSlot.Itemstack == null)
            {
                invSlot.Itemstack = hotbarSlot.Itemstack.Clone();
                invSlot.Itemstack.StackSize = 0;
                Api.World.PlaySoundAt(SoundLocation, Pos.X, Pos.Y, Pos.Z, null, 0.88f + (float)Api.World.Rand.NextDouble() * 0.24f, 16);
            }

            if (invSlot.Itemstack.Equals(Api.World, hotbarSlot.Itemstack, GlobalConstants.IgnoredStackAttributes))
            {
                bool putBulk = player.Entity.Controls.Sprint;

                int q = GameMath.Min(hotbarSlot.StackSize, putBulk ? BulkTakeQuantity : DefaultTakeQuantity, MaxStackSize - OwnStackSize());

                // add to the pile and average item temperatures
                int oldSize = invSlot.Itemstack.StackSize;
                invSlot.Itemstack.StackSize += q;
                if (oldSize + q > 0)
                {
                    float tempPile = invSlot.Itemstack.Collectible.GetTemperature(Api.World, invSlot.Itemstack);
                    float tempAdded = hotbarSlot.Itemstack.Collectible.GetTemperature(Api.World, hotbarSlot.Itemstack);
                    invSlot.Itemstack.Collectible.SetTemperature(Api.World, invSlot.Itemstack, (tempPile * oldSize + tempAdded * q) / (oldSize + q), false);
                }

                if (player.WorldData.CurrentGameMode != EnumGameMode.Creative)
                {
                    hotbarSlot.TakeOut(q);
                    hotbarSlot.OnItemSlotModified(null);
                }

                Api.World.PlaySoundAt(SoundLocation, Pos.X, Pos.Y, Pos.Z, player, 0.88f + (float)Api.World.Rand.NextDouble() * 0.24f, 16);

                MarkDirty();

                Cuboidf[] collBoxes = Api.World.BlockAccessor.GetBlock(Pos).GetCollisionBoxes(Api.World.BlockAccessor, Pos);
                if (collBoxes != null && collBoxes.Length > 0 && CollisionTester.AabbIntersect(collBoxes[0], Pos.X, Pos.Y, Pos.Z, player.Entity.CollisionBox, player.Entity.SidedPos.XYZ))
                {
                    player.Entity.SidedPos.Y += collBoxes[0].Y2 - (player.Entity.SidedPos.Y - (int)player.Entity.SidedPos.Y);
                }

                IClientPlayer clientPlayer = player as IClientPlayer;
                if (clientPlayer != null)
                {
                    clientPlayer.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
                }

                return true;
            }

            return false;
        }

        public bool TryTakeItem(IPlayer player)
        {
            bool takeBulk = player.Entity.Controls.Sprint;
            int q = GameMath.Min(takeBulk ? BulkTakeQuantity : DefaultTakeQuantity, OwnStackSize());

            if (inventory[0].Itemstack != null)
            {
                ItemStack stack = inventory[0].TakeOut(q);
                player.InventoryManager.TryGiveItemstack(stack);

                if (stack.StackSize > 0)
                {
                    Api.World.SpawnItemEntity(stack, Pos.ToVec3d().Add(0.5, 0.5, 0.5));
                }
            }

            if (OwnStackSize() == 0)
            {
                Api.World.BlockAccessor.SetBlock(0, Pos);
            }

            Api.World.PlaySoundAt(SoundLocation, Pos.X, Pos.Y, Pos.Z, player, 0.88f + (float)Api.World.Rand.NextDouble() * 0.24f, 16);

            MarkDirty();
            IClientPlayer clientPlayer = player as IClientPlayer;
            if (clientPlayer != null)
            {
                clientPlayer.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
            }

            return true;
        }

    }
}
