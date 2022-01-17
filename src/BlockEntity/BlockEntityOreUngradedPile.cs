using nrw.frese.stonepile.basics;
using Vintagestory.API.Common;

namespace nrw.frese.stonepile.blockentity
{
    public class BlockEntityOreUngradedPile : BlockEntityPile
    {
        public override string BlockCode { get { return "orepile-ungraded"; } }
        public override int MaxStackSize { get { return 64; } }
        public override int DefaultTakeQuantity { get { return 8; } }
        public override int BulkTakeQuantity { get { return 8; } }
        public override AssetLocation SoundLocation { get { return new AssetLocation("sounds/block/rock-break-pickaxe"); } }
    }
}
