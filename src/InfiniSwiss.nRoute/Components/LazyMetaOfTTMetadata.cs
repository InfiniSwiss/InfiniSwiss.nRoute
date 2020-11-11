using System;

namespace nRoute.Components
{
    /// <remarks>from MEF http://mef.codeplex.com/SourceControl/changeset/view/34058#484572
    /// to be removed when moving to SL4. </remarks>
    public class Lazy<T, TMetadata> : Lazy<T>
    {
        private readonly TMetadata _metadata;

        public Lazy(TMetadata metadata)
            : base()
        {
            _metadata = metadata;
        }

        public Lazy(TMetadata metadata, bool isThreadSafe)
            : base(isThreadSafe)
        {
            _metadata = metadata;
        }

        public Lazy(Func<T> valueFactory, TMetadata metadata)
            : base(valueFactory)
        {
            _metadata = metadata;
        }

        public Lazy(Func<T> valueFactory, TMetadata metadata, bool isThreadSafe)
            : base(valueFactory, isThreadSafe)
        {
            _metadata = metadata;
        }

        #region Properties

        public TMetadata Metadata
        {
            get { return _metadata; }
        }

        #endregion

    }
}