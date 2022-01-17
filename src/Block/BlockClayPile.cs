using nrw.frese.stonepile.basics;

namespace nrw.frese.stonepile.block
{
    internal class BlockClayPile : BlockPile
    {
        public override string AddStoneLabel { get { return "stonepiles:blockhelp-claypile-addclay"; } }
        public override string RemoveStoneLabel { get { return "stonepiles:blockhelp-claypile-removeclay"; } }
        public override int DefaultAddQuantity { get { return 2; } }
    }
}
