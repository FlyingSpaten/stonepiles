using System;
using nrw.frese.stonepile.basics;
using nrw.frese.stonepile.blockentity;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace nrw.frese.stonepile
{ public class BlockStonePile : BlockPile
    {
        public override string AddStoneLabel { get { return "stonepiles:blockhelp-stonepile-add"; } }
        public override string RemoveStoneLabel { get { return "stonepiles:blockhelp-stonepile-remove"; } }
        public override int DefaultAddQuantity { get { return 2; } }
    }
}
