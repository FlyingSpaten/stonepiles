using nrw.frese.stonepile.basics;
using Vintagestory.API.Common;

namespace nrw.frese.stonepile.blockentity
{
    internal class BlockEntityNuggetPile : BlockEntityPile
    {
        public override string BlockCode { get { return "nuggetpile"; } }
        public override int MaxStackSize { get { return 128; } }
        public override int DefaultTakeQuantity { get { return 8; } }
        public override int BulkTakeQuantity { get { return 8; } }
        public override AssetLocation SoundLocation { get { return new AssetLocation("sounds/block/rock-break-pickaxe"); } }
    }
}
