using nrw.frese.stonepile.basics;

namespace nrw.frese.stonepile.block
{
    internal class BlockFlintPile : BlockPile
    {
        public override string AddStoneLabel { get { return "stonepiles:blockhelp-flintpile-addflint"; } }
        public override string RemoveStoneLabel { get { return "stonepiles:blockhelp-flintpile-removeflint"; } }
        public override int DefaultAddQuantity { get { return 2; } }
    }
}
