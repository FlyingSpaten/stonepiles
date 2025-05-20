using nrw.frese.stonepile.basics;

namespace nrw.frese.stonepile.block
{
    internal class BlockFlintPile : BlockPile
    {
        public override string AddStoneLabel { get { return "stonepiles:blockhelp-stonepile-add"; } }
        public override string RemoveStoneLabel { get { return "stonepiles:blockhelp-stonepile-remove"; } }
        public override int DefaultAddQuantity { get { return 2; } }
    }
}
