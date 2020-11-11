namespace nRoute.Components.Routing
{
    internal sealed class LiteralSubsegment
         : PathSubsegment
    {

        public LiteralSubsegment(string literal)
        {
            this.Literal = literal;
        }

        public string Literal { get; private set; }
    }
}
