﻿using nrw.frese.stonepile.basics;
using Vintagestory.API.Common;

namespace nrw.frese.stonepile.blockentity
{
    internal class BlockEntityFlintPile : BlockEntityPile
    {
        public override string BlockCode { get { return "flintpile"; } }
        public override int MaxStackSize { get { return 16; } }
        public override int DefaultTakeQuantity { get { return 2; } }
        public override int BulkTakeQuantity { get { return 2; } }
        public override AssetLocation SoundLocation { get { return new AssetLocation("sounds/block/rock-break-pickaxe"); } }
    }
}
