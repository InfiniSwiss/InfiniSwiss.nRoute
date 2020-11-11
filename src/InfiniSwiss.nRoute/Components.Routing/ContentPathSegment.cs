using System.Collections.Generic;
using System.Linq;

namespace nRoute.Components.Routing
{
    internal sealed class ContentPathSegment
         : PathSegment
    {

        public ContentPathSegment(IList<PathSubsegment> subsegments)
        {
            this.Subsegments = subsegments;
        }

        public bool IsCatchAll
        {
            get
            {
                return this.Subsegments.Any<PathSubsegment>(seg => ((seg is ParameterSubsegment)
                    && ((ParameterSubsegment)seg).IsCatchAll));
            }
        }

        public IList<PathSubsegment> Subsegments { get; private set; }
    }
}
