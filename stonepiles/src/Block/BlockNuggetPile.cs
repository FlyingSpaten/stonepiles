using nrw.frese.stonepile.basics;

namespace nrw.frese.stonepile.block
{
    internal class BlockNuggetPile : BlockPile
    {
        public override string AddStoneLabel { get { return "stonepiles:blockhelp-nuggetpile-addnugget"; } }
        public override string RemoveStoneLabel { get { return "stonepiles:blockhelp-nuggetpile-removenugget"; } }
        public override int DefaultAddQuantity { get { return 8; } }
    }
}
